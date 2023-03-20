using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
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

        /// <summary>
        /// 激活效果器
        /// </summary>
        public void ActivatedEffectorModel(in EffectorModel effectorModel, in IDArgs summoner, Vector2 spawnPos) {
            var len1 = effectorModel.entitySummonModelArray.Length;
            for (int i = 0; i < len1; i++) {
                var entitySummonModel = effectorModel.entitySummonModelArray[i];
                worldContext.RootDomain.SpawnBy_EntitySummonModel(entitySummonModel, summoner, spawnPos);
            }

            var len2 = effectorModel.entityDestroyModelArray.Length;
            for (int i = 0; i < len2; i++) {
                var entityDestroyModel = effectorModel.entityDestroyModelArray[i];
                worldContext.RootDomain.DestroyBy_EntityDestroyModel(entityDestroyModel, summoner);
            }
        }

    }
}