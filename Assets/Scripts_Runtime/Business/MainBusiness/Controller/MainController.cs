using TiedanSouls.Infra.Facades;

namespace TiedanSouls.Main.Controller {

    public class MainController {

        InfraContext infraContext;

        public MainController() { }

        public void Inject(InfraContext infraContext) {
            this.infraContext = infraContext;
        }

        public void Init() {
            var uiCore = infraContext.UICore;
            var uiSetter = uiCore.Setter;
            uiSetter.LoginPage_Open();
            uiSetter.LoginPage_Binding_StartGame(() => {
                infraContext.EventCenter.Invoke_OnStartGameAct();
                uiSetter.LoginPage_Close();
            });
        }

    }
}