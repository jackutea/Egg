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

            var fsmState = fsm.FSMState;
            if (fsmState == RoleFSMState.None) return;

            // - 状态
            TickAny(role, fsm, dt);
            if (fsmState == RoleFSMState.Idle) Tick_Idle(role, fsm, dt);
            else if (fsmState == RoleFSMState.JumpingUp) Tick_JumpingUp(role, fsm, dt);
            else if (fsmState == RoleFSMState.Casting) Tick_Cast(role, fsm, dt);
            else if (fsmState == RoleFSMState.BeHit) Tick_BeHit(role, fsm, dt);
            else if (fsmState == RoleFSMState.Dying) Tick_Dying(role, fsm, dt);
            // - 控制效果

            var ctrlStatus = fsm.CtrlStatus;
            if (ctrlStatus.Contains(RoleCtrlStatus.SkillMove)) Tick_SkillMove(role, fsm, dt);
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
            if (fsm.FSMState == RoleFSMState.Dying) return;

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
            var stateModel = fsm.IdleStateModel;
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

            // Locomotion
            role.HorizontalFaceTo(inputCom.MoveAxis.x);
            role.TryJumpByInput();
            role.TryMoveByInput();
            role.Fall(dt);

            roleDomain.TryCastSkillByInput(role);
        }

        /// <summary>
        /// 上跳状态
        /// </summary>
        void Tick_JumpingUp(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.JumpingUpStateModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
            }

            stateModel.curFrame++;

            if (stateModel.curFrame < 2) {
                fsm.RemovePositionStatus_OnGround();
                fsm.RemovePositionStatus_StandInCrossPlatform();
            }

            // Locomotion
            role.TryMoveByInput();
            role.Fall(dt);
            role.HorizontalFaceTo(role.InputCom.MoveAxis.x);

            var roleDomain = rootDomain.RoleDomain;
            roleDomain.TryCastSkillByInput(role);

            var posStatus = fsm.PositionStatus;
            if (posStatus.Contains(RolePositionStatus.OnGround)
            || posStatus.Contains(RolePositionStatus.OnCrossPlatform)) {
                fsm.Enter_Idle();
            }
        }

        /// <summary>
        /// 释放技能状态
        /// </summary>
        void Tick_Cast(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.CastingStateModel;
            var skillTypeID = stateModel.CastingSkillTypeID;
            var isCombo = stateModel.IsCombo;
            var skillSlotCom = role.SkillSlotCom;
            _ = skillSlotCom.TryGet(skillTypeID, isCombo, out var castingSkill);
            var roleDomain = rootDomain.RoleDomain;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                roleDomain.FaceToHorizontalPoint(role, stateModel.ChosedPoint);
                role.WeaponSlotCom.Weapon.PlayAnim(castingSkill.WeaponAnimName);
            }

            // 先决条件
            if (stateModel.IsWaitingForMoveEnd) return;

            // 技能逻辑迭代
            if (!castingSkill.TryMoveNext(role.LogicRootPos, role.LogicRotation)) {
                fsm.Enter_Idle();
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

            // Locomotion
            role.TryMoveByInput();

            if (fsm.CtrlStatus != RoleCtrlStatus.SkillMove) role.Fall(dt);

            roleDomain.TryCastSkillByInput(role);
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
                if (stateModel.IsFaceTo) role.HorizontalFaceTo(vel.x);
            } else if (stateModel.curFrame == len) {
                var roleDomain = rootDomain.RoleDomain;
                roleDomain.StopMove(role);

                fsm.RemoveCtrlStatus_SkillMove();

                var castingModel = fsm.CastingStateModel;
                castingModel.SetIsWaitingForMoveEnd(false);
            }
        }

        /// <summary>
        /// 被击退状态
        /// </summary>
        /// 
        void Tick_BeHit(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.BeHitStateModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
            }

            stateModel.curFrame++;
            var curFrame = stateModel.curFrame;
            var beHitDir = stateModel.BeHitDir;

            var moveCom = role.MoveCom;
            var knockBackSpeedArray = stateModel.KnockBackSpeedArray;
            var len1 = knockBackSpeedArray.Length;
            bool canKnockBack = curFrame < len1;
            if (canKnockBack) {
                beHitDir = beHitDir.x > 0 ? Vector2.right : Vector2.left;
                moveCom.SetHorizontalVelocity(beHitDir * knockBackSpeedArray[curFrame]);
            } else if (curFrame == len1) {
                moveCom.StopHorizontalVelocity();
            }

            bool isOnGround = fsm.PositionStatus.Contains(RolePositionStatus.OnGround);
            var knockUpSpeedArray = stateModel.KnockUpSpeedArray;
            var len2 = knockUpSpeedArray.Length;
            bool canKnockUp = curFrame < len2;
            if (canKnockUp) {
                var newV = moveCom.Velocity;
                newV.y = knockUpSpeedArray[curFrame];
                moveCom.SetVerticalVelocity(newV);
            } else if (curFrame == len2) {
                moveCom.StopVerticalVelocity();
            } else if (!isOnGround) {
                role.Fall(dt);
            }

            bool isOver = curFrame >= stateModel.MaintainFrame;
            if (isOver) {
                bool hasKnockUp = len2 > 0;
                if (!hasKnockUp) {
                    fsm.Enter_Idle();
                    return;
                }

                if (isOnGround) fsm.Enter_Idle();
            }
        }

        /// <summary>
        /// 死亡状态
        /// </summary>
        void Tick_Dying(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var roleDomain = rootDomain.RoleDomain;

            var stateModel = fsm.DyingStateModel;
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

        #region [动作状态]

        public void Enter_Dying(RoleEntity role) {
            var roleRepo = worldContext.RoleRepo;
            var fsm = role.FSMCom;
            fsm.Enter_Dying(30);
        }

        #endregion

        #region [位置状态]

        public void AddPositionStatus_OnGround(RoleEntity role) {
            var fsm = role.FSMCom;
            fsm.AddPositionStatus_OnGround();
        }

        public void RemovePositionStatus_OnGround(RoleEntity role) {
            var fsm = role.FSMCom;
            fsm.RemovePositionStatus_OnGround();
        }

        #endregion

    }

}