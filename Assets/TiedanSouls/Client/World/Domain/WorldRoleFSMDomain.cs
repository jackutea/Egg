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

        void TickFSM(RoleEntity role, float dt) {
            var fsmCom = role.FSMCom;

            var fsmState = fsmCom.FSMState;
            if (fsmState == RoleFSMState.None) return;

            TickAny(role, fsmCom, dt);
            if (fsmState == RoleFSMState.Idle) Tick_Idle(role, fsmCom, dt);
            else if (fsmState == RoleFSMState.JumpingUp) Tick_JumpingUp(role, fsmCom, dt);
            else if (fsmState == RoleFSMState.Casting) Tick_Casting(role, fsmCom, dt);
            else if (fsmState == RoleFSMState.BeHit) Tick_BeHit(role, fsmCom, dt);
            else if (fsmState == RoleFSMState.Dying) Tick_Dying(role, fsmCom, dt);
        }

        void TickAny(RoleEntity role, RoleFSMComponent fsmCom, float dt) {
            if (fsmCom.FSMState == RoleFSMState.Dying) return;

            var roleDomain = rootDomain.RoleDomain;

            // 任意状态下的死亡判定
            if (roleDomain.IsRoleDead(role)) {
                roleDomain.TearDownRole(role);
            }

            Tick_Buff(role, fsmCom, dt);
            Tick_CtrlEffect(role, fsmCom, dt);

            if (role.IDCom.ControlType == ControlType.AI) role.AIStrategy.Tick(dt);
        }

        void Tick_Buff(RoleEntity role, RoleFSMComponent fsmCom, float dt) {
            var buffDomain = rootDomain.BuffDomain;

            var buffSlotCom = role.BuffSlotCom;
            var removeList = buffSlotCom.ForeachAndGetRemoveList((buff) => {
                buff.AddCurFrame();
                buffDomain.TryTriggerEffector(buff);
                buffDomain.TryTriggerAttributeEffect(role.AttributeCom, buff);
                role.HudSlotCom.HpBarHUD.SetHpBar(role.AttributeCom.HP, role.AttributeCom.HPMax);
            });

            var buffRepo = worldContext.BuffRepo;
            removeList.ForEach((buff) => {
                buffDomain.RevokeBuff(buff, role.AttributeCom);
                buffSlotCom.TryRemove(buff);
                buffRepo.TryRemoveToPool(buff);
                buff.ResetAll();
            });
        }

        void Tick_CtrlEffect(RoleEntity role, RoleFSMComponent fsmCom, float dt) {
            var ctrlEffectSlotCom = role.CtrlEffectSlotCom;
            ctrlEffectSlotCom.Tick();
        }

        #region [位置状态]

        public void AddPositionStatus_OnGround(RoleEntity role) {
            var fsmCom = role.FSMCom;
            fsmCom.AddPositionStatus_OnGround();
        }

        public void RemovePositionStatus_OnGround(RoleEntity role) {
            var fsmCom = role.FSMCom;
            fsmCom.RemovePositionStatus_OnGround();
        }

        #endregion

        /// <summary>
        /// 闲置状态
        /// </summary>
        void Tick_Idle(RoleEntity role, RoleFSMComponent fsmCom, float dt) {
            var stateModel = fsmCom.IdleStateModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                role.RendererModCom.Anim_PlayIdle();
            }

            var roleDomain = rootDomain.RoleDomain;

            // 拾取武器
            var inputCom = role.InputCom;
            if (inputCom.InputPick) {
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
        void Tick_JumpingUp(RoleEntity role, RoleFSMComponent fsmCom, float dt) {
            var stateModel = fsmCom.JumpingUpStateModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
            }

            stateModel.curFrame++;

            if (stateModel.curFrame < 2) {
                fsmCom.RemovePositionStatus_OnGround();
                fsmCom.RemovePositionStatus_StandInCrossPlatform();
            }

            // Locomotion
            role.TryMoveByInput();
            role.Fall(dt);
            role.HorizontalFaceTo(role.InputCom.MoveAxis.x);

            var roleDomain = rootDomain.RoleDomain;
            roleDomain.TryCastSkillByInput(role);

            var posStatus = fsmCom.PositionStatus;
            if (posStatus.Contains(RolePositionStatus.OnGround)
            || posStatus.Contains(RolePositionStatus.OnCrossPlatform)) {
                fsmCom.Enter_Idle();
            }
        }

        /// <summary>
        /// 释放技能状态
        /// </summary>
        void Tick_Casting(RoleEntity role, RoleFSMComponent fsmCom, float dt) {
            var stateModel = fsmCom.CastingStateModel;
            var castingSkill = stateModel.CastingSkill;
            var isCombo = stateModel.IsCombo;
            var skillSlotCom = role.SkillSlotCom;
            var roleDomain = rootDomain.RoleDomain;

            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
                stateModel.SetCasterRotation(role.LogicRotation);
                roleDomain.FaceToHorizontalPoint(role, stateModel.ChosedPoint);
                role.WeaponSlotCom.Weapon.PlayAnim(castingSkill.WeaponAnimName);
            }

            // 先决条件
            if (stateModel.IsWaitingForMoveEnd) return;

            stateModel.curIndex++;
            var curFrame = stateModel.GetCurFrame();
            var lastFrame = stateModel.GetLastFrame();

            bool hasLastFrameMove = castingSkill.HasSkillMoveCurveModel(lastFrame);
            if (!stateModel.IsCurrentValid()) {
                if (hasLastFrameMove) role.Stop();
                var rendererModCom = role.RendererModCom;
                rendererModCom.Anim_SetSpeed(1);
                fsmCom.Enter_Idle();
                return;
            }

            if (hasLastFrameMove) role.Stop();
            castingSkill.SetCurFrame(curFrame);

            // 技能位移
            bool hasCurFrameMove = false;
            if (curFrame - lastFrame > 1) {
                for (int i = lastFrame + 1; i < curFrame; i++) {
                    hasCurFrameMove = TryApplySkillMove(role, dt, i, true) ? true : hasCurFrameMove;
                }
            }
            hasCurFrameMove = TryApplySkillMove(role, dt, curFrame, false) ? true : hasCurFrameMove;

            if (!hasCurFrameMove) {
                role.TryMoveByInput();
                role.Fall(dt);
            }
            roleDomain.TryCastSkillByInput(role);

            // 技能逻辑迭代
            if (!castingSkill.TryApplyFrame(role.LogicRootPos, role.LogicRotation, curFrame)) {
                var rendererModCom = role.RendererModCom;
                rendererModCom.Anim_SetSpeed(1);
                fsmCom.Enter_Idle();
            }

            // 技能效果器
            if (castingSkill.TryGet_ValidSkillEffectorModel(out var skillEffectorModel)) {
                var effectorTypeID = skillEffectorModel.effectorTypeID;
                if (effectorTypeID != 0) {
                    var effectorDomain = this.rootDomain.EffectorDomain;
                    if (effectorDomain.TrySpawnEffectorModel(effectorTypeID, out var effectorModel)) {
                        var summoner = role.IDCom.ToArgs();
                        var baseRot = role.LogicRotation;
                        var summonPos = role.LogicRootPos + baseRot * skillEffectorModel.offsetPos;

                        this.rootDomain.SpawnBy_EntitySummonModelArray(summonPos, baseRot, summoner, effectorModel.entitySummonModelArray);
                        this.rootDomain.DestroyBy_EntityDestroyModelArray(summoner, effectorModel.entityDestroyModelArray);
                    }
                }
            }

        }

        /// <summary>
        /// 应用技能位移
        /// </summary>
        bool TryApplySkillMove(RoleEntity role, float dt, int frame, bool moveByOffset) {
            var fsmCom = role.FSMCom;
            var castingSkill = fsmCom.CastingStateModel.CastingSkill;
            var castingModel = fsmCom.CastingStateModel;
            if (!castingSkill.TryGetSkillMoveCurveModel(frame, out var skillMoveCurveModel)) {
                return false;
            }

            var moveCurveModel = skillMoveCurveModel.moveCurveModel;
            var moveSpeedArray = moveCurveModel.moveSpeedArray;
            var moveDirArray = moveCurveModel.moveDirArray;
            var len = moveSpeedArray.Length;
            var index = frame - skillMoveCurveModel.startFrame;
            if (index > len) return false;

            var moveCom = role.MoveCom;
            if (index < len) {
                var speed = moveSpeedArray[index];
                var moveDir = moveDirArray[index];
                var casterRotation = castingModel.CasterRotation;
                moveDir = casterRotation * moveDir;
                var vel = moveDir * speed;
                if (moveByOffset) {
                    var pos = role.LogicRootPos + vel * dt;
                    role.SetLogicPos(pos);
                } else {
                    moveCom.SetVelocity(moveDir * speed);
                }
                if (skillMoveCurveModel.isFaceTo) {
                    role.HorizontalFaceTo(vel.x);
                }
            }

            return true;
        }

        /// <summary>
        /// 受击状态
        /// </summary>
        void Tick_BeHit(RoleEntity role, RoleFSMComponent fsmCom, float dt) {
            var stateModel = fsmCom.BeHitStateModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);
            }

            stateModel.curFrame++;
            var curFrame = stateModel.curFrame;
            var beHitDir = stateModel.BeHitDir;

            var moveCom = role.MoveCom;
            var knockBackSpeedArray = stateModel.KnockBackSpeedArray;
            if (knockBackSpeedArray != null) {
                var len = knockBackSpeedArray.Length;
                bool canKnockBack = curFrame < len;
                if (canKnockBack) {
                    beHitDir = beHitDir.x > 0 ? Vector2.right : Vector2.left;
                    moveCom.SetHorizontalVelocity(beHitDir * knockBackSpeedArray[curFrame]);
                } else if (curFrame == len) {
                    moveCom.StopHorizontalVelocity();
                }
            }

            var knockUpSpeedArray = stateModel.KnockUpSpeedArray;
            if (knockUpSpeedArray != null) {
                bool isOnGround = fsmCom.PositionStatus.Contains(RolePositionStatus.OnGround);
                var len = knockUpSpeedArray.Length;
                bool canKnockUp = curFrame < len;
                if (canKnockUp) {
                    var newV = moveCom.Velocity;
                    newV.y = knockUpSpeedArray[curFrame];
                    moveCom.SetVerticalVelocity(newV);
                } else if (curFrame == len) {
                    moveCom.StopVerticalVelocity();
                } else if (!isOnGround) {
                    role.Fall(dt);
                }

                bool isOver = curFrame >= stateModel.MaintainFrame;
                if (isOver) {
                    bool hasKnockUp = len > 0;
                    if (!hasKnockUp) {
                        fsmCom.Enter_Idle();
                        return;
                    }

                    if (isOnGround) fsmCom.Enter_Idle();
                }
            }
        }

        /// <summary>
        /// 死亡状态
        /// </summary>
        void Tick_Dying(RoleEntity role, RoleFSMComponent fsmCom, float dt) {
            var roleDomain = rootDomain.RoleDomain;

            var stateModel = fsmCom.DyingStateModel;
            if (stateModel.IsEntering) {
                stateModel.SetIsEntering(false);

                role.HudSlotCom.HideHUD();
                role.RendererModCom.Anim_Play_Dying();
                role.Stop();
            }

            stateModel.maintainFrame--;
            if (stateModel.maintainFrame <= 0) {
                roleDomain.TearDownRole(role);
            }
        }

        public void Enter_Dying(RoleEntity role) {
            var roleRepo = worldContext.RoleRepo;
            var fsmCom = role.FSMCom;
            fsmCom.Enter_Dying(30);
        }

        public void Enter_Casting(RoleEntity role, SkillEntity skill, bool isCombo) {
            var fsmCom = role.FSMCom;
            fsmCom.Enter_Casting(skill, isCombo, role.InputCom.ChosenPoint);

            var stateModel = fsmCom.CastingStateModel;
            var frameArray = stateModel.FrameArray;
            var skillTotalFrame = skill.TotalFrame;
            var normalSkillSpeedBonus = role.AttributeCom.NormalSkillSpeedBonus;
            int frameCount = Mathf.RoundToInt(1 / (1 + normalSkillSpeedBonus) * skillTotalFrame);
            int cutFrameCount = skillTotalFrame - frameCount;
            int realCutFrameCount = 0;

            const short KEY_FRAME_TAG = -1;
            const short CUT_FRAME_TAG = -2;
            const short NORMAL_FRAME_TAG = 0;
            for (int i = 0; i < skillTotalFrame; i++) {
                if (skill.IsKeyFrame(i)) {
                    // 关键帧
                    frameArray[i] = KEY_FRAME_TAG;
                } else if (realCutFrameCount < cutFrameCount) {
                    // 裁剪帧 
                    frameArray[i] = CUT_FRAME_TAG;
                    realCutFrameCount++;
                } else {
                    // 普通帧
                    frameArray[i] = NORMAL_FRAME_TAG;
                }
            }

            frameArray.CutElementsAndLeftMove_CoverByIndex(0, skillTotalFrame - 1, CUT_FRAME_TAG);
            int realFrameCount = skillTotalFrame - realCutFrameCount;
            stateModel.SetFrameCount(realFrameCount);

            var rendererModCom = role.RendererModCom;
            rendererModCom.Anim_SetSpeed(skillTotalFrame / (float)realFrameCount);

            var str = $" =====================\n\t ";
            for (int i = 0; i < realFrameCount; i++) {
                str += frameArray[i] + "\t";
            }
            TDLog.Log(str);
        }

    }

}