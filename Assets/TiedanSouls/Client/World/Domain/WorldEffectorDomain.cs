using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldEffectorDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldEffectorDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        public bool TryGetEffectorModel(int typeID, out EffectorModel effectorModel) {
            effectorModel = default;

            var effectorTemplate = infraContext.TemplateCore.EffectorTemplate;
            if (!effectorTemplate.TryGet(typeID, out var tm)) {
                return false;
            }

            effectorModel = TM2ModelUtil.GetModel_Effector(tm);
            // TODO: 把EffectorModel用Repo存储起来，避免每次都要重新创建
            return true;
        }

    }
}