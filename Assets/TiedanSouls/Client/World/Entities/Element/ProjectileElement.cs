using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 弹道元素模型
    /// </summary>
    public class ProjectileElement : MonoBehaviour {

        IDArgs father;
        public IDArgs Father => father;

        #region [组件]

        InputComponent inputCom;
        public InputComponent InputCom => inputCom;

        AttributeComponent attributeCom;
        public AttributeComponent AttributeCom => attributeCom;

        MoveComponent moveCom;
        public MoveComponent MoveCom => moveCom;

        ProjectileElementFSMComponent fsmCom;
        public ProjectileElementFSMComponent FSMCom => fsmCom;

        #endregion

        #region [逻辑层]

        Transform logicRoot;
        public Transform LogicRoot => logicRoot;
        public Vector2 GetPos_LogicRoot() => logicRoot.position;

        Rigidbody2D rb_logicRoot;

        #endregion

        #region [表现层]

        Transform rendererRoot;
        public Transform RendererRoot => rendererRoot;
        public Vector2 GetPos_RendererRoot() => rendererRoot.position;

        GameObject vfxGO;
        public GameObject VFXGO => vfxGO;
        public void SetVFXGO(GameObject value) => this.vfxGO = value;

        #endregion

        #region [杂项]

        Vector2 bornPos;
        public Vector2 BornPos => bornPos;
        public void SetBornPos(Vector2 value) => this.bornPos = value;

        sbyte faceDirX;
        public sbyte FaceDirX => faceDirX;

        int fromFieldTypeID;
        public int FromFieldTypeID => fromFieldTypeID;
        public void SetFromFieldTypeID(int value) => this.fromFieldTypeID = value;

        #endregion

        #region [生命周期]

        int startFrame;
        public int StartFrame => startFrame;
        public void SetStartFrame(int value) => startFrame = value;

        int endFrame;
        public int EndFrame => endFrame;
        public void SetEndFrame(int value) => endFrame = value;

        #endregion

        #region [碰撞器]

        CollisionTriggerModel triggerModel;
        public CollisionTriggerModel TriggerModel => triggerModel;
        public void SetTriggerModel(in CollisionTriggerModel value) => this.triggerModel = value;

        #endregion

        #region [效果器]

        // - 打击效果器
        EffectorModel hitEffectorModel;
        public EffectorModel HitEffectorModel => hitEffectorModel;
        public void SetHitEffectorModel(EffectorModel value) => this.hitEffectorModel = value;

        // 死亡效果器
        EffectorModel deadEffectorModel;
        public EffectorModel DeadEffectorModel => deadEffectorModel;
        public void SetDeadEffectorModel(EffectorModel value) => this.deadEffectorModel = value;

        #endregion

        #region [额外打击次数]

        int extraHitTimes;
        public int ExtraHitTimes => extraHitTimes;
        public void SetExtraHitTime(int value) => extraHitTimes = value;
        public void ReduceHitExtraTimes() => extraHitTimes--;

        #endregion

        public void Ctor() {
            faceDirX = 1;

            // Root
            logicRoot = transform.Find("logic_root");
            rb_logicRoot = logicRoot.GetComponent<Rigidbody2D>();
            rendererRoot = transform.Find("renderer_root");

            // Component
            moveCom = new MoveComponent();
            moveCom.Inject(rb_logicRoot);
            inputCom = new InputComponent();
            attributeCom = new AttributeComponent();
            fsmCom = new ProjectileElementFSMComponent();
        }

        public void TearDown() {
            GameObject.Destroy(gameObject);
        }

        public void Reset() {
            attributeCom.Reset();
            inputCom.Reset();
        }

        public void Show() {
            logicRoot.gameObject.SetActive(true);
            rendererRoot.gameObject.SetActive(true);
        }

        public void Hide() {
            logicRoot.gameObject.SetActive(false);
            rendererRoot.gameObject.SetActive(false);
        }

        public void Attribute_HP_Decrease(int atk) {
            attributeCom.HP_Decrease(atk);
        }

        public Vector3 GetPos_Logic() {
            return logicRoot.position;
        }

        public float GetAngleZ_Logic() {
            return logicRoot.rotation.z;
        }

        public Quaternion GetRot_Logic() {
            return logicRoot.rotation;
        }

        public void SetPos_Logic(Vector2 pos) {
            logicRoot.position = pos;
        }

        #region [表现同步]

        public void Renderer_Sync() {
            var logicPos = logicRoot.position;
            rendererRoot.position = logicPos;
        }

        public void Renderer_Lerp(float dt) {
            var lerpPos = Vector3.Lerp(rendererRoot.position, logicRoot.position, dt * 30);
            rendererRoot.position = lerpPos;
        }

        #endregion

    }

}