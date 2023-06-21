using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Generic;
using TiedanSouls.Client.Entities;

namespace TiedanSouls.Client.Domain {

    public class WorldEffectorDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldEffectorDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public bool TrySpawnEffectorModel(int typeID, out EffectorEntity effectorModel) {
            effectorModel = default;
            if (typeID == 0) return false;

            effectorModel = default;

            var effectorTemplate = infraContext.TemplateCore.EffectorTemplate;
            if (!effectorTemplate.TryGet(typeID, out var tm)) {
                TDLog.Error($"找不到效果器模板 - {typeID}");
                return false;
            }

            // TODO: 把 EffectorModel用Repo存储起来，避免每次都要重新创建
            effectorModel = TM2ModelUtil.GetRoleEffectorEntity(tm);
            TDLog.Log($"生成效果器 ======> {effectorModel.effectorName}");
            
            return true;
        }

    }
}