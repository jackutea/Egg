using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldRoleFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain rootDomain;

        public WorldRoleFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = worldDomain;
        }

        public void TickFSM(int curFieldTypeID, float dt) {
            worldContext.RoleRepo.Foreach_AI(curFieldTypeID, (role) => {
                TickFSM(role, dt);
            });

            var playerRole = worldContext.RoleRepo.PlayerRole;
            if (playerRole != null) {
                TickFSM(playerRole, dt);
            }
        }

        #region [角色状态 - Tick]

        void TickFSM(RoleEntity role, float dt) {
            var fsm = role.FSMCom;

            var actionState = fsm.ActionState;
            if (actionState == RoleActionState.None) return;

            // - 动作 状态
            if (actionState == RoleActionState.Idle) Tick_Idle(role, fsm, dt);
            else if (actionState == RoleActionState.Casting) Tick_Cast(role, fsm, dt);
            else if (actionState == RoleActionState.Dying) Tick_Dying(role, fsm, dt);
            TickAny(role, fsm, dt);

            // - 控制 状态
            var ctrlStatus = fsm.CtrlStatus;
            if (ctrlStatus.Contains(RoleCtrlStatus.KnockUp)) Tick_KnockUp(role, fsm, dt);
            if (ctrlStatus.Contains(RoleCtrlStatus.SkillMove)) Tick_SkillMove(role, fsm, dt);
            if (ctrlStatus.Contains(RoleCtrlStatus.KnockBack)) Tick_KnockBack(role, fsm, dt);

            // - 位置 状态 TODO
            var positionStatus = fsm.PositionStatus;

            Apply_Locomotion(role, fsm, dt);    // 移动
            Apply_RealseSkill(role, fsm, dt);   // 释放技能
        }

        void Tick_Buff(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var buffDomain = rootDomain.BuffDomain;

            var buffSlotCom = role.BuffSlotCom;
            var removeList = buffSlotCom.ForeachAndGetRemoveList((buff) => {
                buff.curFrame++;
                buffDomain.TryTriggerEffector(buff);
                buffDomain.TryTriggerAttributeEffect(role.AttributeCom, buff);
                role.HudSlotCom.HpBarHUD.SetHpBar(role.AttributeCom.HP, role.AttributeCom.HPMax);
            });

            removeList.ForEach((buffEntityID) => {
                buffSlotCom.RemoveByEntityID(buffEntityID);
                buffDomain.RevokeBuffByEntityID(buffEntityID, role.AttributeCom);
            });
        }

        void TickAny(RoleEntity role, RoleFSMComponent fsm, float dt) {
            if (fsm.ActionState == RoleActionState.Dying) return;

            var roleDomain = rootDomain.RoleDomain;

            // 任意状态下的死亡判定
            if (roleDomain.IsRoleDead(role)) {
                roleDomain.TearDownRole(role);
            }

            Tick_Buff(role, fsm, dt);

            if (role.IDCom.ControlType == ControlType.AI) role.AIStrategy.Tick(dt);
        }

        /// <summary>
        /// 闲置状态
        /// </summary>
        void Tick_Idle(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.IdleModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                role.RendererModCom.Anim_PlayIdle();
            }

            var roleDomain = rootDomain.RoleDomain;

            // 拾取武器
            var inputCom = role.InputCom;
            if (inputCom.PressPick) {
                roleDomain.TryPickUpSomethingFromField(role);
            }
        }

        /// <summary>
        /// 释放技能状态
        /// </summary>
        void Tick_Cast(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.CastingModel;
            var skillTypeID = stateModel.CastingSkillTypeID;
            var isCombo = stateModel.IsCombo;
            var skillSlotCom = role.SkillSlotCom;
            _ = skillSlotCom.TryGet(skillTypeID, isCombo, out var castingSkill);
            var roleDomain = rootDomain.RoleDomain;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                roleDomain.FaceTo_Horizontal(role, stateModel.ChosedPoint);
                role.WeaponSlotCom.Weapon.PlayAnim(castingSkill.WeaponAnimName);
            }

            // 先决条件
            if (stateModel.IsWaitingForMoveEnd) return;

            // 技能逻辑迭代
            if (!castingSkill.TryMoveNext(role.LogicRootPos, role.LogicRotation)) {
                fsm.EnterActionState_Idle();
            }

            // 技能效果器
            if (castingSkill.TryGet_ValidSkillEffectorModel(out var skillEffectorModel)) {
                var effectorTypeID = skillEffectorModel.effectorTypeID;
                if (effectorTypeID != 0) {
                    var effectorDomain = this.rootDomain.EffectorDomain;
                    if (!effectorDomain.TrySpawnEffectorModel(effectorTypeID, out var effectorModel)) {
                        Debug.LogWarning($"请检查配置! 效果器没有找到! 类型ID {effectorTypeID}");
                        return;
                    }

                    var summoner = role.IDCom.ToArgs();
                    var baseRot = role.LogicRotation;
                    var summonPos = role.LogicRootPos + baseRot * skillEffectorModel.offsetPos;

                    this.rootDomain.SpawnBy_EntitySummonModelArray(summonPos, baseRot, summoner, effectorModel.entitySummonModelArray);
                    this.rootDomain.DestroyBy_EntityDestroyModelArray(summoner, effectorModel.entityDestroyModelArray);
                }
            }

            // 技能位移
            if (castingSkill.TryGet_ValidSkillMoveCurveModel(out var skillMoveCurveModel)) {
                fsm.AddCtrlStatus_SkillMove(skillMoveCurveModel);
                stateModel.SetIsWaitingForMoveEnd(skillMoveCurveModel.needWaitForMoveEnd);
            }
        }

        /// <summary>
        /// 技能位移状态
        /// </summary>
        void Tick_SkillMove(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.SkillMoveModel;
            var moveDirArray = stateModel.MoveDirArray;
            var moveSpeedArray = stateModel.MoveSpeedArray;
            var len = moveSpeedArray.Length;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                // 位移方向 初始化
                var baseRot = role.LogicRotation;
                for (int i = 0; i < len; i++) {
                    var moveDir = moveDirArray[i];
                    moveDirArray[i] = baseRot * moveDir;
                }
            }

            stateModel.curFrame++;

            var moveCom = role.MoveCom;
            if (stateModel.curFrame < len) {
                var speed = moveSpeedArray[stateModel.curFrame];
                var moveDir = moveDirArray[stateModel.curFrame];
                var vel = moveDir * speed;
                moveCom.SetVelocity(moveDir * speed);
                if (stateModel.IsFaceTo) role.SetLogicFaceTo(vel.x);
            } else if (stateModel.curFrame == len) {
                var roleDomain = rootDomain.RoleDomain;
                roleDomain.StopMove(role);

                fsm.RemoveCtrlStatus_SkillMove();

                var castingModel = fsm.CastingModel;
                castingModel.SetIsWaitingForMoveEnd(false);
            }
        }

        /// <summary>
        /// 被击退状态
        /// </summary>
        void Tick_KnockBack(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.KnockBackModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
            }

            stateModel.curFrame++;
            var curFrame = stateModel.curFrame;

            var moveCom = role.MoveCom;
            var beHitDir = stateModel.beHitDir;
            var knockBackSpeedArray = stateModel.knockBackSpeedArray;
            var len = knockBackSpeedArray.Length;
            bool canKnockBack = curFrame < len;
            if (canKnockBack) {
                beHitDir = beHitDir.x > 0 ? Vector2.right : Vector2.left;
                moveCom.SetHorizontalVelocity(beHitDir * knockBackSpeedArray[curFrame]);
            } else if (curFrame == len) {
                moveCom.StopHorizontalVelocity();
                fsm.RemoveCtrlStatus_KnockBack();
            }
        }

        /// <summary>
        /// 被击飞状态
        /// </summary>
        void Tick_KnockUp(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.KnockUpModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
            }

            stateModel.curFrame++;
            var curFrame = stateModel.curFrame;

            var moveCom = role.MoveCom;

            var roleDomain = rootDomain.RoleDomain;
            var knockUpSpeedArray = stateModel.knockUpSpeedArray;
            var len = knockUpSpeedArray.Length;
            bool canKnockUp = curFrame < len;
            if (canKnockUp) {
                var newV = moveCom.Velocity;
                newV.y = knockUpSpeedArray[curFrame];
                moveCom.SetVerticalVelocity(newV);
            } else if (curFrame == len) {
                moveCom.StopVerticalVelocity();
                fsm.RemoveCtrlStatus_KnockUp();
            }

        }

        /// <summary>
        /// 死亡状态
        /// </summary>
        void Tick_Dying(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var roleDomain = rootDomain.RoleDomain;

            var stateModel = fsm.DyingModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);

                role.HudSlotCom.HideHUD();
                role.RendererModCom.Anim_Play_Dying();
                roleDomain.StopMove(role);
            }

            stateModel.maintainFrame--;
            if (stateModel.maintainFrame <= 0) {
                roleDomain.TearDownRole(role);
            }
        }

        #endregion

        #region [角色各项处理] 

        /// <summary>
        /// 处理 运动状态
        /// </summary>
        void Apply_Locomotion(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var roleDomain = rootDomain.RoleDomain;
            if (fsm.Can_Move()) roleDomain.TryMoveByInput(role);
            if (fsm.CanJumpDown()) roleDomain.TryCrossDownPlatformByInput(role);
            if (fsm.CanJumpUp()) roleDomain.JumpByInput(role);
            if (fsm.CanFall()) roleDomain.Fall(role, dt);
            if (fsm.CanChangeFaceTo()) roleDomain.FaceToMoveDir(role);
        }

        /// <summary>
        /// 处理 技能释放
        /// </summary>
        void Apply_RealseSkill(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var roleDomain = rootDomain.RoleDomain;

            // 普通技能
            if (fsm.CanCastNormalSkill()) {
                _ = roleDomain.TryCastSkillByInput(role);
            }

            // TODO: 觉醒技能........
        }

        #endregion

        #region [动作状态 切换]

        public void Enter_Dying(RoleEntity role) {
            var roleRepo = worldContext.RoleRepo;
            var fsm = role.FSMCom;
            fsm.EnterActionState_Dying(30);
        }

        public void Enter_KnockBack(RoleEntity role, Vector3 beHitDir, in KnockBackModel knockBackModel) {
            var knockBackSpeedArray = knockBackModel.knockBackSpeedArray;
            if (knockBackSpeedArray == null || knockBackSpeedArray.Length == 0) {
                return;
            }
            var fsm = role.FSMCom;
            fsm.AddCtrlStatus_KnockBack(beHitDir, knockBackModel);
        }

        public void Enter_KnockUp(RoleEntity role, Vector3 beHitDir, in KnockUpModel knockUpModel) {
            var knockUpSpeedArray = knockUpModel.knockUpSpeedArray;
            if (knockUpSpeedArray == null || knockUpSpeedArray.Length == 0) {
                return;
            }
            var fsm = role.FSMCom;
            fsm.AddCtrlStatus_KnockUp(knockUpModel);
        }

        #endregion

    }

}