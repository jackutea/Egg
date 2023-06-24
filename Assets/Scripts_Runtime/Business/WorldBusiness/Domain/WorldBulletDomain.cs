using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldBulletDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldBulletDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        /// <summary>
        /// 根据类型ID生成子弹
        /// </summary>
        public bool TryGetOrCreate(int typeID, in EntityIDComponent father, out BulletEntity bullet) {
            bullet = null;

            var bulletRepo = worldContext.BulletRepo;
            bool isFromPool = bulletRepo.TryFetchFromPool(typeID, out bullet);
            if (!isFromPool) {
                var factory = worldContext.Factory;
                if (!factory.TryCreateBullet(typeID, out bullet)) {
                    TDLog.Error($"实体生成<子弹>失败 类型ID:{typeID}");
                    return false;
                }
            }

            // ID
            var idCom = bullet.IDCom;
            if (isFromPool) {
                bullet.Reset();
            } else {
                var entityID = worldContext.IDService.PickBulletID();
                idCom.SetEntityID(entityID);
            }
            idCom.SetFather(father);

            // 碰撞盒关联
            bulletRepo.Add(bullet);

            return true;
        }

        public void RecycleFieldBullets(int fieldTypeID) {
            var repo = worldContext.BulletRepo;
            repo.RecycleToPool(fieldTypeID);
        }

        /// <summary>
        /// 子弹 击中 逻辑
        /// </summary>
        public void HandleHit(BulletEntity bullet) {
            if (!bullet.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            bullet.ReduceExtraPenetrateCount();

            // 子弹击中效果器
            var rootDomain = worldContext.RootDomain;
            var effectorDomain = rootDomain.EffectorDomain;
            var roleDomain = rootDomain.RoleDomain;
            var buffDomain = rootDomain.BuffDomain;

            object fatherHolder = bullet.IDCom.Father.HolderPtr;

            if (fatherHolder is RoleEntity role) {
                var selfRoleEffectorTypeIDArray = collisionTriggerModel.selfRoleEffectorTypeIDArray;
                var len = selfRoleEffectorTypeIDArray.Length;
                for (int i = 0; i < len; i++) {
                    var effectorTypeID = selfRoleEffectorTypeIDArray[i];
                    if (!effectorDomain.TrySpawnEffectorModel(effectorTypeID, out var effectorModel)) continue;

                    var attributeCom = role.AttributeCom;
                    if (!attributeCom.IsMatch(effectorModel.roleEffectorModel.roleSelectorModel)) continue;

                    roleDomain.ModifyRole(role.AttributeCom, effectorModel.roleEffectorModel.roleModifyModel, 1);
                }
            }
        }

        /// <summary>
        /// 子弹 受击 逻辑
        /// </summary>
        public void HandleBeHit(BulletEntity bullet) {
            if (!bullet.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }
        }

        /// <summary>
        /// 根据 实体追踪模型 设置 第一个满足条件的实体目标
        /// </summary>
        public void TrySetEntityTrackTarget(ref EntityTrackModel entityTrackModel, in EntityIDComponent self) {
            var stateEntity = this.worldContext.StateEntity;
            var curFieldTypeID = stateEntity.CurFieldTypeID;

            var entityTrackSelectorModel = entityTrackModel.entityTrackSelectorModel;
            var trackEntityType = entityTrackSelectorModel.entityType;
            var attributeSelectorModel = entityTrackSelectorModel.attributeSelectorModel;

            if (trackEntityType == EntityType.Role) {
                var roleRepo = this.worldContext.RoleRepo;
                if (roleRepo.TryGet_TrackEntity(
                    curFieldTypeID,
                    entityTrackModel.relativeTrackTargetGroupType,
                    self,
                    attributeSelectorModel,
                    out var role
                )) {
                    entityTrackModel.target = role.IDCom;
                }

                return;
            }

            TDLog.Error($"EntityTrack 未处理的实体类型 {trackEntityType}");
            return;
        }

        /// <summary>
        /// 子弹追踪目标
        /// </summary>
        public void MoveToTrackingTarget(BulletEntity bullet) {
            var entityTrackModel = bullet.entityTrackModel;
            var target = entityTrackModel.target;
            if (target.EntityType == EntityType.None) {
                // 说明没有目标了
                return;
            }

            var entityTrackSelectorModel = entityTrackModel.entityTrackSelectorModel;
            var entityType = entityTrackSelectorModel.entityType;
            var trackSpeed = entityTrackModel.trackSpeed;
            Vector3 targetPos;
            if (entityType == EntityType.Role) {
                var targetRole = worldContext.RoleRepo.GetByID(target.EntityID);
                targetPos = targetRole.MoveCom.Pos;
            } else if (entityType == EntityType.Bullet) {
                worldContext.BulletRepo.TryGetByID(target.EntityID, out var targetBullet);
                targetPos = targetBullet.MoveCom.Pos;
            } else {
                TDLog.Error($"子弹追踪目标类型错误! - {entityType}");
                return;
            }

            var moveCom = bullet.MoveCom;
            var bulletPos = moveCom.Pos;
            var posOffset = targetPos - bulletPos;
            if (posOffset.sqrMagnitude < 0.01f) {
                // 说明已经到达目标点了
                return;
            }

            var moveDir = posOffset.normalized;
            var velocity = moveDir * trackSpeed;
            moveCom.SetVelocity(velocity);

            var rot = Quaternion.FromToRotation(Vector3.right, moveDir);
            bullet.SetLogicRotation(rot);
        }

        public void MoveStraight(BulletEntity bullet) {
            var fsmCom = bullet.FSMCom;
            var model = fsmCom.ActivatedModel;
            var curFrame = model.curFrame;
            var moveCom = bullet.MoveCom;
            // 直线
            if (bullet.TryGetMoveSpeed(curFrame, out var speed)) {
                _ = bullet.TryGetMoveDir(curFrame, out var moveDir);
                var realMoveDir = bullet.BaseRotation * moveDir;
                var velocity = realMoveDir * speed;
                moveCom.SetVelocity(velocity);

                var rot = Quaternion.FromToRotation(Vector3.right, realMoveDir);
                bullet.SetLogicRotation(rot);

            } else if (bullet.IsJustPassLastMoveFrame(curFrame)) {
                moveCom.Stop();
            }
        }

    }

}