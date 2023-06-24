using UnityEngine;
using TiedanSouls.Infra.Facades;
using TiedanSouls.Client.Facades;
using TiedanSouls.Client.Entities;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Domain {

    public class WorldSkillDomain {

        InfraContext infraContext;
        WorldContext worldContext;

        public WorldSkillDomain() { }

        public void Inject(InfraContext infraContext, WorldContext worldContext) {
            this.infraContext = infraContext;
            this.worldContext = worldContext;
        }

        #region [击中 & 受击]

        /// <summary>
        /// 技能 击中 处理
        /// </summary>
        public void HandleHit(SkillEntity skill, Vector3 skillColliderPos, in ColliderToggleModel collisionTriggerModel) {
            var rootDomain = worldContext.RootDomain;
            var roleDomain = rootDomain.RoleDomain;
            var effectorDomain = rootDomain.EffectorDomain;
            var buffDomain = rootDomain.BuffDomain;

            // 打击触发自身 角色效果器
            var skillIDCom = skill.IDCom;
            object fatherHolder = skillIDCom.Father.HolderPtr;
            RoleEntity selfRole = fatherHolder as RoleEntity;
            if (selfRole == null) {
                TDLog.Error($"技能击中时, 未找到技能拥有者! - {skillIDCom}");
                return;
            }

            var selfRoleEffectorTypeIDArray = collisionTriggerModel.selfRoleEffectorTypeIDArray;
            var len = selfRoleEffectorTypeIDArray?.Length;
            for (int i = 0; i < len; i++) {
                var effectorTypeID = selfRoleEffectorTypeIDArray[i];
                if (!effectorDomain.TrySpawnEffectorModel(effectorTypeID, out var effectorModel)) continue;

                var roleSelectorModel = effectorModel.roleEffectorModel.roleSelectorModel;
                var roleModifyModel = effectorModel.roleEffectorModel.roleModifyModel;
                var roleAttrCom = selfRole.AttributeCom;
                if (!roleAttrCom.IsMatch(roleSelectorModel)) continue;

                roleDomain.ModifyRole(selfRole.AttributeCom, roleModifyModel, 1);
            }
        }

        /// <summary>
        /// 技能 受击 处理
        /// </summary>
        public void HandleBeHit(SkillEntity skill, in ColliderToggleModel collisionTriggerModel, int hitFrame) {

        }

        #endregion

    }

}