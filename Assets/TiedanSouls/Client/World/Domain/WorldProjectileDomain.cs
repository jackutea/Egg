using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldProjectileDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain rootDomain;

        public WorldProjectileDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.rootDomain = worldDomain;
        }

        public bool TrySummonProjectile(Vector3 pos, Quaternion spawnRot, in IDArgs summoner, in EntitySummonModel entitySummonModel, out ProjectileEntity projectile) {
            var typeID = entitySummonModel.typeID;
            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateProjectile(typeID, out projectile)) {
                TDLog.Error($"创建实体 弹道 失败! - {typeID}");
                return false;
            }

            // ID
            var idCom = projectile.IDCom;
            idCom.SetEntityID(worldContext.IDService.PickFieldID());

            // Father
            idCom.SetFather(summoner);

            var projectileBulletModelArray = projectile.ProjectileBulletModelArray;
            var len = projectileBulletModelArray.Length;
            for (int i = 0; i < len; i++) {
                var bulletModel = projectileBulletModelArray[i];
                var bullet = bulletModel.bulletEntity;
                // 碰撞盒关联
                this.rootDomain.SetFather_CollisionTriggerModel(bullet.CollisionTriggerModel, idCom.ToArgs());
                // TODO  元素 位置&角度 
            }

            // TODO: 走配置，不一定立刻激活，可能需要等待一段时间，或者等待某个条件满足
            var fsm = projectile.FSMCom;
            fsm.Enter_Activated();

            // Repo
            var repo = worldContext.ProjectileRepo;
            repo.Add(projectile);

            return true;
        }
    }

}