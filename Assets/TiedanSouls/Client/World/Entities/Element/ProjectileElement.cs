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
        
        GameObject logicGO;
        public GameObject LogicGO => logicGO;

        #endregion

        #region [表现层]

        GameObject vfxGO;
        public GameObject VFXGO => vfxGO;
        public void SetVFXGO(GameObject value) => this.vfxGO = value;

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
            logicGO = new GameObject("弹道元素");
            rb = logicGO.AddComponent<Rigidbody2D>();

            // Component
            moveCom = new MoveComponent();
            moveCom.Inject(rb);
            inputCom = new InputComponent();
            attributeCom = new AttributeComponent();
            fsmCom = new ProjectileElementFSMComponent();
        }

        public void TearDown() {
            logicGO.SetActive(false);
            vfxGO.gameObject.SetActive(false);
        }

        public void SetFather(in IDArgs father) {
            this.father = father;
        }

        public void Reset() {
            attributeCom.Reset();
            inputCom.Reset();
        }

        public void Activated() {
            logicGO.SetActive(true);
            vfxGO.SetActive(true);
        }

        public void Deactivated() {
            logicGO.SetActive(false);
            vfxGO.SetActive(false);
        }

        public void Attribute_HP_Decrease(int atk) {
            attributeCom.HP_Decrease(atk);
        }

        public Vector3 GetLogic_Pos() {
            return logicGO.transform.position;
        }

        public float GetLogic_AngleZ() {
            return logicGO.transform.rotation.z;
        }

        public Quaternion GetLogic_Rot() {
            return logicGO.transform.rotation;
        }

        public void SetElementPos(Vector2 pos) {
            logicGO.transform.position = pos;
        }

        #region [表现同步]

        public void Renderer_Sync() {
            var elementPos = logicGO.transform.position;
            vfxGO.transform.position = elementPos;
        }

        public void Renderer_Lerp(float dt) {
            var vfxPos = vfxGO.transform.position;
            var elementPos = logicGO.transform.position;
            var lerpPos = Vector3.Lerp(vfxPos, elementPos, dt * 30);
            vfxGO.transform.position = lerpPos;
        }

        #endregion

    }

}