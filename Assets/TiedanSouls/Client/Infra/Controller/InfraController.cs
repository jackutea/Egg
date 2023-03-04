using System.Threading.Tasks;
using UnityEngine;
using TiedanSouls.Infra.Facades;

namespace TiedanSouls.Infra.Controller {

    public class InfraController {

        InfraContext infraContext;

        public InfraController() { }

        public void Inject(Canvas canvas, InfraContext infraContext) {
            this.infraContext = infraContext;
            var uiCore = infraContext.UICore;
            uiCore.Inject(canvas);
        }

        public async Task Init() {

            var assetCore = infraContext.AssetCore;
            await assetCore.Init();

            var templateCore = infraContext.TemplateCore;
            await templateCore.Init();

            var uiCore = infraContext.UICore;
            await uiCore.Init();

            var cameraCore = infraContext.CameraCore;
            cameraCore.Initialize(Camera.main);
            cameraCore.SetterAPI.SpawnByMain(1);

            var inputCore = infraContext.InputCore;
            var inputSetter = inputCore.Setter;

            inputSetter.Bind(InputKeyCollection.MOVE_LEFT, KeyCode.A);
            inputSetter.Bind(InputKeyCollection.MOVE_RIGHT, KeyCode.D);
            inputSetter.Bind(InputKeyCollection.MOVE_UP, KeyCode.W);
            inputSetter.Bind(InputKeyCollection.MOVE_DOWN, KeyCode.S);

            inputSetter.Bind(InputKeyCollection.JUMP, KeyCode.Space);
            inputSetter.Bind(InputKeyCollection.JUMP, KeyCode.K);

            inputSetter.Bind(InputKeyCollection.MELEE, KeyCode.Mouse0);
            inputSetter.Bind(InputKeyCollection.MELEE, KeyCode.J);

            inputSetter.Bind(InputKeyCollection.SPEC_MELEE, KeyCode.Mouse1);
            inputSetter.Bind(InputKeyCollection.SPEC_MELEE, KeyCode.I);

            inputSetter.Bind(InputKeyCollection.BOOM_MELEE, KeyCode.Mouse2);
            inputSetter.Bind(InputKeyCollection.BOOM_MELEE, KeyCode.U);

            inputSetter.Bind(InputKeyCollection.INFINITY, KeyCode.F);
            inputSetter.Bind(InputKeyCollection.INFINITY, KeyCode.O);

            inputSetter.Bind(InputKeyCollection.DASH, KeyCode.LeftShift);
            inputSetter.Bind(InputKeyCollection.DASH, KeyCode.L);

            inputSetter.Bind(InputKeyCollection.PICK, KeyCode.E);

            inputSetter.Bind(InputKeyCollection.CHOOSE_POINT, KeyCode.Mouse0);
            inputSetter.Bind(InputKeyCollection.CHOOSE_POINT, KeyCode.Mouse1);
            inputSetter.Bind(InputKeyCollection.CHOOSE_POINT, KeyCode.J);

        }

        public void Tick(float dt) {

        }

        public void LateTick(float dt) {
            var cameraCore = infraContext.CameraCore;
            cameraCore.Tick(dt);
        }

    }

}