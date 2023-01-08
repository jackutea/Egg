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
            var assetGetter = infraContext.AssetCore.Getter;
            bool has = assetGetter.TryGetWorldAsset("entity_field", out GameObject go);
            if (!has) {
                TDLog.Error("Failed to get asset: entity_field");
                return null;
            }
            var entity = GameObject.Instantiate(go).GetComponent<FieldEntity>();
            entity.Ctor();
            return entity;
        }

        public RoleEntity CreateRoleEntity(IDService idService) {
            var assetGetter = infraContext.AssetCore.Getter;
            bool has = assetGetter.TryGetWorldAsset("entity_role", out GameObject go);
            if (!has) {
                TDLog.Error("Failed to get asset: entity_role");
                return null;
            }
            var entity = GameObject.Instantiate(go).GetComponent<RoleEntity>();
            entity.Ctor();
            
            int id = idService.PickRoleID();
            entity.SetID(id);

            return entity;
        }

    }

}