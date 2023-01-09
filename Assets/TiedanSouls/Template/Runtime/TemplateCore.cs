using System.Threading.Tasks;

namespace TiedanSouls.Template {

    public class TemplateCore {

        RoleTemplate roleTemplate;
        public RoleTemplate RoleTemplate => roleTemplate;

        SkillorTemplate skillorTemplate;
        public SkillorTemplate SkillorTemplate => skillorTemplate;

        public TemplateCore() {
            roleTemplate = new RoleTemplate();
            skillorTemplate = new SkillorTemplate();
        }

        public async Task Init() {
            await roleTemplate.LoadAll();
            await skillorTemplate.LoadAll();
        }

    }

}