using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 弹道元素模型
    /// </summary>
    public class ProjectileElement {

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

        #region [GameObject]

        Rigidbody2D rb;

        GameObject rootGO;
        public GameObject RootGO => rootGO;

        GameObject logicRoot;
        public GameObject LogicRoot => logicRoot;

        GameObject vfxRoot;
        public GameObject VFXRoot => vfxRoot;

        #endregion

        #region [表现层]

        GameObject vfxGO;
        public GameObject VFXGO => vfxGO;

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

        CollisionTriggerModel collisionTriggerModel;
        public CollisionTriggerModel CollisionTriggerModel => collisionTriggerModel;
        public void SetCollisionTriggerModel(in CollisionTriggerModel value) => this.collisionTriggerModel = value;

        #endregion

        #region [效果器]

        // - 打击效果器
        EffectorModel hitEffectorModel;
        public EffectorModel HitEffectorModel => hitEffectorModel;
        public void SetHitEffectorModel(EffectorModel value) => this.hitEffectorModel = value;

        // 死亡效果器
        EffectorModel deathEffectorModel;
        public EffectorModel DeathEffectorModel => deathEffectorModel;
        public void SetDeathEffectorModel(EffectorModel value) => this.deathEffectorModel = value;

        #endregion

        #region [额外打击次数]

        int extraHitTimes;
        public int ExtraHitTimes => extraHitTimes;
        public void SetExtraHitTime(int value) => extraHitTimes = value;
        public void ReduceHitExtraTimes() => extraHitTimes--;

        #endregion

        Vector2 bornPos;
        public Vector2 BornPos => bornPos;
        public void SetBornPos(Vector2 value) => this.bornPos = value;

        public void Ctor() {
            // GameObject
            rootGO = new GameObject("弹道元素");
            logicRoot = new GameObject("LogicRoot");
            vfxRoot = new GameObject("VFXRoot");
            logicRoot.transform.SetParent(rootGO.transform, false);
            vfxRoot.transform.SetParent(rootGO.transform, false);

            // Rb
            rb = logicRoot.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;

            // Component
            moveCom = new MoveComponent();
            moveCom.Inject(rb);
            inputCom = new InputComponent();
            attributeCom = new AttributeComponent();
            fsmCom = new ProjectileElementFSMComponent();
        }

        public void TearDown() {
            rootGO.SetActive(false);
            vfxGO.gameObject.SetActive(false);
        }

        public void SetFather(in IDArgs father) {
            this.father = father;
        }

        public void SetVFXGO(GameObject value) {
            value.transform.SetParent(vfxRoot.transform, false);
            this.vfxGO = value;
        }

        public void Reset() {
            attributeCom.Reset();
            inputCom.Reset();
        }

        public void Activated() {
            rootGO.SetActive(true);
            vfxGO.SetActive(true);
        }

        public void Deactivated() {
            rootGO.SetActive(false);
            vfxGO.SetActive(false);
        }

        public void Attribute_HP_Decrease(int atk) {
            attributeCom.HP_Decrease(atk);
        }

        public Vector3 GetLogic_Pos() {
            return rootGO.transform.position;
        }

        public float GetLogic_AngleZ() {
            return rootGO.transform.rotation.z;
        }

        public Quaternion GetLogic_Rot() {
            return rootGO.transform.rotation;
        }

        public void SetElementPos(Vector2 pos) {
            rootGO.transform.position = pos;
        }

        #region [表现同步]

        public void Renderer_Sync() {
            var elementPos = rootGO.transform.position;
            vfxGO.transform.position = elementPos;
        }

        public void Renderer_Lerp(float dt) {
            var vfxPos = vfxGO.transform.position;
            var elementPos = rootGO.transform.position;
            var lerpPos = Vector3.Lerp(vfxPos, elementPos, dt * 30);
            vfxGO.transform.position = lerpPos;
        }

        #endregion

    }

}