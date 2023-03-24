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
        public bool TrySpawnBullet(int typeID, out BulletEntity bullet) {
            bullet = null;

            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateBullet(typeID, out bullet)) {
                TDLog.Error($"生成子弹失败 typeID:{typeID}");
                return false;
            }

            // 1. 子弹 ID
            var idCom = bullet.IDCom;
            idCom.SetEntityID(worldContext.IDService.PickFieldID());

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
            idCom.SetEntityID(worldContext.IDService.PickFieldID());
            idCom.SetFromFieldTypeID(fromFieldTypeID);
            idCom.SetControlType(controlType);
            idCom.SetAllyType(allyType);

            // 2. 子弹 碰撞盒关联
            this.rootDomain.SetFather_CollisionTriggerModel(bullet.CollisionTriggerModel, idCom.ToArgs());

            // 3. 添加至仓库
            var repo = worldContext.BulletRepo;
            repo.Add(bullet);
            return true;
        }

    }

}