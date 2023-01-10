using System.Threading.Tasks;

namespace TiedanSouls.Template {

    public class TemplateCore {

        RoleTemplate roleTemplate;
        public RoleTemplate RoleTemplate => roleTemplate;

        SkillorTemplate skillorTemplate;
        public SkillorTemplate SkillorTemplate => skillorTemplate;

        WeaponTemplate weaponTemplate;
        public WeaponTemplate WeaponTemplate => weaponTemplate;

        public TemplateCore() {
            roleTemplate = new RoleTemplate();
            skillorTemplate = new SkillorTemplate();
            weaponTemplate = new WeaponTemplate();
        }

        public async Task Init() {
            await roleTemplate.LoadAll();
            await skillorTemplate.LoadAll();
            await weaponTemplate.LoadAll();
        }

    }

}