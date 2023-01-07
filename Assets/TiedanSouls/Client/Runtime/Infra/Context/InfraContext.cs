using GameArki.PlatformerCamera;
using GameArki.FreeInput;
using UIRenderer;
using TiedanSouls.Asset;

namespace TiedanSouls.Infra.Facades {

    public class InfraContext {

        PFCore cameraCore;
        public PFCore CameraCore => cameraCore;

        FreeInputCore inputCore;
        public FreeInputCore InputCore => inputCore;

        UICore uiCore;
        public UICore UICore => uiCore;

        InfraEventCenter eventCenter;
        public InfraEventCenter EventCenter => eventCenter;

        AssetCore assetCore;
        public AssetCore AssetCore => assetCore;

        public InfraContext() {
            cameraCore = new PFCore();
            inputCore = new FreeInputCore();
            uiCore = new UICore();
            eventCenter = new InfraEventCenter();
            assetCore = new AssetCore();
        }

    }

}