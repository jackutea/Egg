using UnityEngine;
using TiedanSouls.Asset;
using TiedanSouls.Infra.Facades;
using TiedanSouls.World.Entities;
using TiedanSouls.World.Service;

namespace TiedanSouls.World {

    public class WorldFactory {

        InfraContext infraContext;

        public WorldFactory() { }

        public void Inject(InfraContext infraContext) {
            this.infraContext = infraContext;
        }

        public FieldEntity CreateFieldEntity() {
            var worldAssets = infraContext.AssetCore.WorldAssets;
            bool has = worldAssets.TryGet("entity_field", out GameObject go);
            if (!has) {
                TDLog.Error("Failed to get asset: entity_field");
                return null;
            }
            var entity = GameObject.Instantiate(go).GetComponent<FieldEntity>();
            entity.Ctor();
            return entity;
        }

    }

}