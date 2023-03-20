using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Domain {

    public class WorldProjectileDomain {

        InfraContext infraContext;
        WorldContext worldContext;
        WorldRootDomain worldRootDomain;

        public WorldProjectileDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext, WorldRootDomain worldDomain) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
            this.worldRootDomain = worldDomain;
        }

        public bool TrySpawnProjectile(ControlType controlType, int typeID, in IDArgs summoner, Vector2 pos, out ProjectileEntity projectile) {
            var factory = worldContext.WorldFactory;
            if (!factory.TrySpawnProjectile(typeID, out projectile)) {
                TDLog.Error($"创建实体 弹道 失败! - {typeID}");
                return false;
            }

            // Pos
            projectile.SetBornPos(pos);
            projectile.SetPos(pos);

            // ID
            var idCom = projectile.IDCom;
            idCom.SetEntityID(worldContext.IDService.PickFieldID());

            // Father
            idCom.SetFather(summoner);

            // Repo
            var repo = worldContext.ProjectileRepo;
            repo.Add(projectile);

            return true;
        }
    }

}