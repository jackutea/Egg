using System.Threading.Tasks;

namespace TiedanSouls.Template {

    public class TemplateCore {

        SkillorTemplate skillorTemplate;
        public SkillorTemplate SkillorTemplate => skillorTemplate;

        public TemplateCore() {
            skillorTemplate = new SkillorTemplate();
        }

        public async Task Init() {
            await skillorTemplate.LoadAll();
        }

    }

}