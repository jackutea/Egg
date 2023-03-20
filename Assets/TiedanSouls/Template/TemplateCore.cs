using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using TiedanSouls.Generic;

namespace TiedanSouls.Template {

    public class TemplateCore {

        #region [实体模板]

        RoleTemplate roleTemplate;
        public RoleTemplate RoleTemplate => roleTemplate;

        SkillTemplate skillTemplate;
        public SkillTemplate SkillTemplate => skillTemplate;

        WeaponTemplate weaponTemplate;
        public WeaponTemplate WeaponTemplate => weaponTemplate;

        FieldTemplate fieldTemplate;
        public FieldTemplate FieldTemplate => fieldTemplate;

        ItemTemplate itemTemplate;
        public ItemTemplate ItemTemplate => itemTemplate;

        ProjectileTemplate projectileTemplate;
        public ProjectileTemplate ProjectileTemplate => projectileTemplate;

        #endregion

        #region [Misc]

        AITemplate aiTemplate;
        public AITemplate AITemplate => aiTemplate;

        EffectorTemplate effectorTemplate;
        public EffectorTemplate EffectorTemplate => effectorTemplate;

        GameConfigTM gameConfigTM;
        public GameConfigTM GameConfigTM => gameConfigTM;

        #endregion

        public TemplateCore() {
            roleTemplate = new RoleTemplate();
            skillTemplate = new SkillTemplate();
            weaponTemplate = new WeaponTemplate();
            fieldTemplate = new FieldTemplate();
            itemTemplate = new ItemTemplate();
            projectileTemplate = new ProjectileTemplate();

            aiTemplate = new AITemplate();
            effectorTemplate = new EffectorTemplate();
        }

        public async Task Init() {
            await roleTemplate.LoadAll();
            await skillTemplate.LoadAll();
            await weaponTemplate.LoadAll();
            await fieldTemplate.LoadAll();
            await itemTemplate.LoadAll();
            await projectileTemplate.LoadAll();
            
            await aiTemplate.LoadAll();
            await effectorTemplate.LoadAll();

            // GameConfig
            AssetLabelReference label = new AssetLabelReference();
            label.labelString = AssetsLabelCollection.SO_CONFIG;
            var gameConfigSO = await Addressables.LoadAssetAsync<GameConfigSO>(label).Task;
            this.gameConfigTM = gameConfigSO.tm;
        }

    }

}