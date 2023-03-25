using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldBulletDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain rootDomain;

        public WorldBulletDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = worldDomain;
        }

        /// <summary>
        /// 根据类型ID生成子弹
        /// </summary>
        public bool TrySpawnBullet(int typeID, in IDArgs father, out BulletEntity bullet) {
            bullet = null;

            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateBullet(typeID, out bullet)) {
                TDLog.Error($"生成子弹失败 typeID:{typeID}");
                return false;
            }

            // 1. 子弹 ID
            var idCom = bullet.IDCom;
            idCom.SetEntityID(worldContext.IDService.PickBulletID());
            idCom.SetFather(father);

            // 2. 子弹 碰撞盒关联
            this.rootDomain.SetFather_CollisionTriggerModel(bullet.CollisionTriggerModel, idCom.ToArgs());

            // 3. 添加至仓库
            var repo = worldContext.BulletRepo;
            repo.Add(bullet);

            return true;
        }

        /// <summary>
        /// 根据实体生成模型 生成子弹
        /// </summary>
        public bool TrySpawnBullet_BySpawnModel(int fromFieldTypeID, in EntitySpawnModel entitySpawnModel, out BulletEntity bullet) {
            bullet = null;

            var typeID = entitySpawnModel.typeID;
            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateBullet(typeID, out bullet)) {
                TDLog.Error($"生成子弹失败 typeID:{typeID}");
                return false;
            }

            var controlType = entitySpawnModel.controlType;
            var allyType = entitySpawnModel.allyType;
            var pos = entitySpawnModel.spawnPos;

            // 1. 子弹 ID
            var idCom = bullet.IDCom;
            idCom.SetEntityID(worldContext.IDService.PickBulletID());
            idCom.SetFromFieldTypeID(fromFieldTypeID);
            idCom.SetAllyType(allyType);
            idCom.SetControlType(controlType);

            // 2. 子弹 碰撞盒关联
            this.rootDomain.SetFather_CollisionTriggerModel(bullet.CollisionTriggerModel, idCom.ToArgs());

            // 3. 添加至仓库
            var repo = worldContext.BulletRepo;
            repo.Add(bullet);
            return true;
        }

        /// <summary>
        /// 子弹击中时触发的逻辑
        /// </summary>
        public void HandleBeHit(BulletEntity bullet) {
            if (!bullet.TryGet_ValidCollisionTriggerModel(out var collisionTriggerModel)) {
                return;
            }

            TriggerHitEffector(bullet, collisionTriggerModel);
            bullet.ReduceExtraPenetrateCount();
        }

        /// <summary>
        /// 触发子弹的 击中效果器
        /// </summary>
        public void TriggerHitEffector(BulletEntity bullet, in CollisionTriggerModel collisionTriggerModel) {
            var effectorTypeID = collisionTriggerModel.hitEffectorTypeID;
            TriggerEffector(bullet, effectorTypeID);
        }

        /// <summary>
        /// 触发子弹的 死亡效果器
        /// </summary>
        public void TriggerDeathEffector(BulletEntity bullet) {
            var effectorTypeID = bullet.DeathEffectorTypeID;
            TriggerEffector(bullet, effectorTypeID);
        }

        void TriggerEffector(BulletEntity bullet, int effectorTypeID) {
            var effectorDomain = rootDomain.EffectorDomain;
            if (!effectorDomain.TrySpawnEffectorModel(effectorTypeID, out var effectorModel)) {
                return;
            }
            var summonPos = bullet.LogicPos;
            var baseRot = bullet.LogicRotation;
            var summoner = bullet.IDCom.ToArgs();
            var entitySummonModelArray = effectorModel.entitySummonModelArray;
            var entityDestroyModelArray = effectorModel.entityDestroyModelArray;
            this.rootDomain.SpawnBy_EntitySummonModelArray(summonPos, baseRot, summoner, entitySummonModelArray);
            this.rootDomain.DestroyBy_EntityDestroyModelArray(summoner, entityDestroyModelArray);
        }

        /// <summary>
        /// 子弹受击处理
        /// </summary>
        public void HandleBeHit(BulletEntity bullet, in CollisionTriggerModel collisionTriggerModel, int hitFrame) {

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

            this.rootDomain.TryGetEntityObj(target, out var entity);
            this.rootDomain.TryGetEntityPos(entity, out var targetPos);
            var moveCom = bullet.MoveCom;
            var bulletPos = moveCom.Pos;
            var moveDir = (targetPos - bulletPos).normalized;
            var velocity = moveDir * trackSpeed;
            moveCom.SetVelocity(velocity);

            var rot = Quaternion.FromToRotation(Vector3.right, moveDir);
            bullet.SetLogicRotation(rot);
        }

        public void MoveStraight(BulletEntity bullet) {
            var fsm = bullet.FSMCom;
            var model = fsm.ActivatedModel;
            var curFrame = model.curFrame;
            var moveCom = bullet.MoveCom;
            // 直线
            if (bullet.TryGetMoveSpeed(curFrame, out var speed)) {
                _ = bullet.TryGetMoveDir(curFrame, out var moveDir);
                var velocity = moveDir * speed;
                moveCom.SetVelocity(velocity);
            } else if (bullet.IsJustPassLastMoveFrame(curFrame)) {
                moveCom.Stop();
            }
        }

    }

}