using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldItemDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldItemDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public void SpawnItemEntity(int itemTypeID, Vector2 pos, in IDArgs father) {
            var factory = worldContext.WorldFactory;
            if (!factory.TryCreateItemEntity(itemTypeID, out var item)) {
                TDLog.Error($"创建物品失败! - {itemTypeID}");
                return;
            }

            item.SetPos(pos);

            // Father
            var idCom = item.IDCom;
            idCom.SetFather(father);

            var repo = worldContext.ItemRepo;
            repo.Add(item);

            TDLog.Log($"生成实体 物件 - {idCom}");
        }

    }

}