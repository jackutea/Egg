using GameArki.PlatformerCamera;
using GameArki.FreeInput;
using UIRenderer;

namespace TiedanSouls.Infra.Facades {

    public class InfraContext {

        PFCore cameraCore;
        public PFCore CameraCore => cameraCore;

        FreeInputCore inputCore;
        public FreeInputCore InputCore => inputCore;

        UICore uiCore;
        public UICore UICore => uiCore;

        public InfraContext() {
            cameraCore = new PFCore();
            inputCore = new FreeInputCore();
            uiCore = new UICore();
        }

    }

}