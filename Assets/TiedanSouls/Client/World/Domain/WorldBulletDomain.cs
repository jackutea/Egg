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
        public bool TryGetOrCreate(int typeID, in EntityIDArgs father, out BulletEntity bullet) {
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
            var rootDomain = worldContext.RootDomain;
            rootDomain.SetEntityColliderTriggerModelFather(bullet.CollisionTriggerModel, idCom.ToArgs());

            bulletRepo.Add(bullet);

            return true;
        }

        /// <summary>
        /// 根据实体生成模型 生成子弹
        /// </summary>
        public bool TrySpawn(int fromFieldTypeID, in EntitySpawnModel entitySpawnModel, out BulletEntity bullet) {
            bullet = null;

            var typeID = entitySpawnModel.typeID;
            var factory = worldContext.Factory;
            if (!factory.TryCreateBullet(typeID, out bullet)) {
                TDLog.Error($"生成子弹失败 typeID:{typeID}");
                return false;
            }
            bullet.SetFromFieldTypeID(fromFieldTypeID);

            var spawnPos = entitySpawnModel.spawnPos;
            var spawnControlType = entitySpawnModel.controlType;
            var spawnAllyType = entitySpawnModel.campType;

            // 1. 子弹 ID
            var idCom = bullet.IDCom;
            idCom.SetEntityID(worldContext.IDService.PickBulletID());
            idCom.SetAllyType(spawnAllyType);
            idCom.SetControlType(spawnControlType);

            // 2. 子弹 碰撞盒关联
            var rootDomain = worldContext.RootDomain;
            rootDomain.SetEntityColliderTriggerModelFather(bullet.CollisionTriggerModel, idCom.ToArgs());

            // 3. 添加至仓库
            var repo = worldContext.BulletRepo;
            repo.Add(bullet);
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

            // TODO : 子弹击中效果器
            bullet.ReduceExtraPenetrateCount();
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
        /// 子弹追踪目标
        /// </summary>
        public void MoveToTrackingTarget(BulletEntity bullet) {
            var entityTrackModel = bullet.entityTrackModel;
            var target = entityTrackModel.target;
            if (target.entityType == EntityType.None) {
                // 说明没有目标了
                return;
            }

            var entityTrackSelectorModel = entityTrackModel.entityTrackSelectorModel;
            var entityType = entityTrackSelectorModel.entityType;
            var trackSpeed = entityTrackModel.trackSpeed;

            var rootDomain = worldContext.RootDomain;
            rootDomain.TryGetEntityObj(target, out var entity);
            rootDomain.TryGetEntityPos(entity, out var targetPos);
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