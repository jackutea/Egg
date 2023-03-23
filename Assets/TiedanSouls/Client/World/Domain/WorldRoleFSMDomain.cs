using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldRoleFSMDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldDomain;

        public WorldRoleFSMDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldDomain = worldDomain;
        }

        public void TickFSM(int curFieldTypeID, float dt) {
            worldContext.RoleRepo.Foreach_AI(curFieldTypeID, (role) => {
                var fsm = role.FSMCom;
                if (fsm.IsExiting) return;

                if (fsm.StateFlag != StateFlag.Dying) {
                    role.AIStrategy.Tick(dt);
                }

                TickFSM(role, dt);

                if (role.IDCom.AllyType == AllyType.Two) role.HudSlotCom.HpBarHUD.SetColor(Color.red);
                else if (role.IDCom.AllyType == AllyType.Neutral) role.HudSlotCom.HpBarHUD.SetColor(Color.yellow);
            });

            var playerRole = worldContext.RoleRepo.PlayerRole;
            if (playerRole != null) {
                TickFSM(playerRole, dt);
            }
        }

        void TickFSM(RoleEntity role, float dt) {
            var fsm = role.FSMCom;
            if (fsm.IsExiting) return;

            // - 1. Tick 状态
            var stateFlag = fsm.StateFlag;
            if (stateFlag.Contains(StateFlag.Idle)) Tick_Idle(role, fsm, dt);
            if (stateFlag.Contains(StateFlag.Cast)) Tick_Cast(role, fsm, dt);
            if (stateFlag.Contains(StateFlag.KnockUp)) Tick_KnockUp(role, fsm, dt);
            if (stateFlag.Contains(StateFlag.KnockBack)) Tick_KnockBack(role, fsm, dt);
            if (stateFlag.Contains(StateFlag.Dying)) Tick_Dying(role, fsm, dt);
            Tick_AnyState(role, fsm, dt);

            // - 2. Apply 各项处理
            Apply_Locomotion(role, fsm, dt);    // 移动
            Apply_RealseSkill(role, fsm, dt);   // 释放技能
        }

        #region [角色状态处理]

        /// <summary>
        /// 任意状态
        /// </summary>
        void Tick_AnyState(RoleEntity role, RoleFSMComponent fsm, float dt) {
            if (fsm.StateFlag == StateFlag.Dying) return;

            var roleDomain = worldDomain.RoleDomain;

            // 任意状态下的死亡判定
            if (roleDomain.IsRoleDead(role)) {
                fsm.AddDying(30);
            }

            // 任意状态下的Idle设置判定
            if (fsm.NeedSetIdle()) {
                fsm.SetIdle();
            }
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

            var roleDomain = worldDomain.RoleDomain;

            // 拾取武器
            var inputCom = role.InputCom;
            if (inputCom.HasInput_Basic_Pick) {
                roleDomain.TryPickUpSomethingFromField(role);
            }

            // 面向移动方向
            roleDomain.FaceToMoveDir(role);
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
            var roleDomain = worldDomain.RoleDomain;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                roleDomain.FaceTo_Horizontal(role, stateModel.ChosedPoint);
                role.WeaponSlotCom.Weapon.PlayAnim(castingSkill.WeaponAnimName);
            }

            // 技能效果器
            if (castingSkill.TryGet_ValidSkillEffectorModel(out var skillEffectorModel)) {
                var triggerFrame = skillEffectorModel.triggerFrame;
                var effectorTypeID = skillEffectorModel.effectorTypeID;
                var effectorDomain = worldDomain.EffectorDomain;
                if (!effectorDomain.TryGetEffectorModel(effectorTypeID, out var effectorModel)) {
                    Debug.LogError($"请检查配置! 效果器没有找到! 类型ID {effectorTypeID}");
                    return;
                }

                var idArgs = role.IDCom.ToArgs();
                var offsetPos = skillEffectorModel.offsetPos;
                var spawnRot = role.GetRot_Logic();
                var spawnPos = role.GetPos_Logic() + spawnRot * offsetPos;
                effectorDomain.ActivatedEffectorModel(effectorModel, idArgs, spawnPos, spawnRot);
            }

            // 技能逻辑迭代
            if (!castingSkill.TryMoveNext(role.GetPos_Logic(), role.GetRot_Logic())) {
                fsm.RemoveCast();
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

            var curFrame = stateModel.curFrame;
            var moveCom = role.MoveCom;
            var beHitDir = stateModel.beHitDir;
            var knockBackSpeedArray = stateModel.knockBackSpeedArray;
            var len = knockBackSpeedArray.Length;
            bool canKnockBack = curFrame < len;
            if (canKnockBack) {
                beHitDir = beHitDir.x > 0 ? Vector2.right : Vector2.left;
                var newV = beHitDir * knockBackSpeedArray[curFrame];
                var oldV = moveCom.Velocity;
                moveCom.SetVelocity(newV);
            } else if (curFrame == len) {
                moveCom.StopHorizontal();
                fsm.RemoveKnockBack();
            }

            stateModel.curFrame++;
        }

        /// <summary>
        /// 被击飞状态
        /// </summary>
        void Tick_KnockUp(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.KnockUpModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
            }

            var curFrame = stateModel.curFrame;
            var moveCom = role.MoveCom;

            var roleDomain = worldDomain.RoleDomain;
            var knockUpSpeedArray = stateModel.knockUpSpeedArray;
            var len = knockUpSpeedArray.Length;
            bool canKnockUp = curFrame < len;
            if (canKnockUp) {
                var newV = moveCom.Velocity;
                newV.y = knockUpSpeedArray[curFrame];
                moveCom.SetVelocity(newV);
            } else if (curFrame == len) {
                moveCom.StopVertical();
                fsm.RemoveKnockUp();
            } else {
                roleDomain.Fall(role, dt);
            }
        }

        /// <summary>
        /// 死亡状态
        /// </summary>
        void Tick_Dying(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.DyingModel;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);

                role.HudSlotCom.HideHUD();
                role.RendererModCom.Anim_Play_Dying();
                role.MoveCom.Stop();
            }

            var roleDomain = worldDomain.RoleDomain;
            roleDomain.Fall(role, dt);

            stateModel.maintainFrame--;
            if (stateModel.maintainFrame <= 0) {
                roleDomain.Role_Die(role);
            }
        }

        #endregion

        #region [角色各项处理] 

        /// <summary>
        /// 处理 运动状态
        /// </summary>
        void Apply_Locomotion(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var roleDomain = worldDomain.RoleDomain;
            if (fsm.CanMove()) roleDomain.MoveByInput(role);
            if (fsm.CanJump()) roleDomain.JumpByInput(role);
            if (fsm.CanFall()) roleDomain.Fall(role, dt);
        }

        /// <summary>
        /// 处理 技能释放
        /// </summary>
        void Apply_RealseSkill(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var roleDomain = worldDomain.RoleDomain;

            // 普通技能
            if (fsm.CanCast_NormalSkill()) {
                _ = roleDomain.TryCastSkillByInput(role);
            }

            // TODO: 觉醒技能........
        }

        #endregion

    }
}