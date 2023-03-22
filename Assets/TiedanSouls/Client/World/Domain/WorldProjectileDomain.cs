using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

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

        public bool TrySpawnProjectile(ControlType controlType, int typeID, in IDArgs summoner, Vector3 pos, Quaternion spawnRot, out ProjectileEntity projectile) {
            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateProjectile(typeID, out projectile)) {
                TDLog.Error($"创建实体 弹道 失败! - {typeID}");
                return false;
            }

            // 弹道 位置&角度 
            projectile.SetBornPos(pos);
            projectile.SetPos(pos);

            // ID
            var idCom = projectile.IDCom;
            idCom.SetEntityID(worldContext.IDService.PickFieldID());

            // Father
            idCom.SetFather(summoner);

            // 元素 位置&角度 
            var rootElement = projectile.RootElement;
            rootElement.SetRotation_UseRelativeOffset(spawnRot);
            rootElement.SetPos_UseRelativeOffset(pos);

            var leafElementArray = projectile.LeafElementArray;
            var len = leafElementArray.Length;
            for (int i = 0; i < len; i++) {
                var leafElement = leafElementArray[i];
                leafElement.SetRotation_UseRelativeOffset(spawnRot);
                leafElement.SetPos_UseRelativeOffset(pos);
            }

            // 立刻激活  、  TODO: 走配置，不一定立刻激活，可能需要等待一段时间，或者等待某个条件满足
            var fsm = projectile.FSMCom;
            fsm.Enter_Activated();

            projectile.name = $"弹道 {projectile.IDCom}";

            // Repo
            var repo = worldContext.ProjectileRepo;
            repo.Add(projectile);

            return true;
        }
    }

}