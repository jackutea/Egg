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
        /// 根据实体生成模型 生成子弹
        /// </summary>
        public bool TrySpawnBullet(int fromFieldTypeID, in EntitySpawnModel entitySpawnModel, out BulletEntity bullet) {
            bullet = null;

            var typeID = entitySpawnModel.typeID;
            var pos = entitySpawnModel.spawnPos;
            var allyType = entitySpawnModel.allyType;
            var controlType = entitySpawnModel.controlType;

            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateBullet(typeID, out bullet)) {
                TDLog.Error($"生成子弹失败 typeID:{typeID}");
                return false;
            }

            // var repo = infraContext.BulletRepo;//TODO
            return true;
        }

    }

}