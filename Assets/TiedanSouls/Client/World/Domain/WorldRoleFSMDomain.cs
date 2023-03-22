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

            Apply_Idle(role, fsm, dt);
            Apply_Cast(role, fsm, dt);
            Apply_KnockBack(role, fsm, dt);
            Apply_Dying(role, fsm, dt);
            Apply_AnyState(role, fsm, dt);
        }

        void Apply_AnyState(RoleEntity role, RoleFSMComponent fsm, float dt) {
            if (fsm.StateFlag == StateFlag.Dying) return;
            var roleDomain = worldDomain.RoleDomain;
            if (roleDomain.IsRoleDead(role)) {
                role.FSMCom.Add_Dying(30);
            }
        }

        void Apply_Idle(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.IdleModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                role.RendererModCom.Anim_PlayIdle();
            }

            var roleDomain = worldDomain.RoleDomain;

            roleDomain.Move_Horizontal(role);
            roleDomain.Jump(role);
            roleDomain.Falling(role, dt);
            roleDomain.CrossDown(role);

            // 拾取武器
            var inputCom = role.InputCom;
            if (inputCom.HasInput_Basic_Pick) {
                roleDomain.TryPickUpSomethingFromField(role);
            }

            // 面向移动方向
            roleDomain.FaceToMoveDir(role);

            // 释放技能
            _ = roleDomain.TryCastSkillByInput(role);
        }

        void Apply_Cast(RoleEntity role, RoleFSMComponent fsm, float dt) {
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

            // 释放技能
            if (roleDomain.TryCastSkillByInput(role)) {
                return;
            }

            // Locomotion
            roleDomain.Move_Horizontal(role);
            roleDomain.Jump(role);
            roleDomain.Falling(role, dt);

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
                fsm.Add_Idle();
            }
        }

        void Apply_KnockBack(RoleEntity role, RoleFSMComponent fsm, float dt) {
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
            if (canKnockBack) KnockBack(moveCom, beHitDir, knockBackSpeedArray[curFrame]);
            else if (curFrame == len) {
                moveCom.StopHorizontal();
                fsm.Remove_KnockBack();
            }

            stateModel.curFrame++;
        }

        void Apply_KnockUp(RoleEntity role, RoleFSMComponent fsm, float dt) {
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
                KnockUp(moveCom, knockUpSpeedArray[curFrame]);
            } else if (curFrame == len) {
                moveCom.StopVertical();
                fsm.Remove_KnockUp();
            } else {
                roleDomain.Falling(role, dt);
            }
        }

        void KnockBack(MoveComponent moveCom, Vector2 beHitDir, float speed) {
            beHitDir = beHitDir.x > 0 ? Vector2.right : Vector2.left;
            var newV = beHitDir * speed;
            var oldV = moveCom.Velocity;
            moveCom.SetVelocity(newV);
        }

        void KnockUp(MoveComponent moveCom, float speed) {
            var newV = moveCom.Velocity;
            newV.y = speed;
            moveCom.SetVelocity(newV);
        }

        void Apply_Dying(RoleEntity role, RoleFSMComponent fsm, float dt) {
            var stateModel = fsm.DyingModel;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);

                role.HudSlotCom.HideHUD();
                role.RendererModCom.Anim_Play_Dying();
                role.MoveCom.Stop();
            }

            var roleDomain = worldDomain.RoleDomain;
            roleDomain.Falling(role, dt);

            stateModel.maintainFrame--;
            if (stateModel.maintainFrame <= 0) {
                roleDomain.Role_Die(role);
            }
        }

    }
}