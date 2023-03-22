using TiedanSouls.Generic;
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

        GameObject rendererRoot;
        public GameObject RendererRoot => rendererRoot;

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

        EffectorModel hitEffectorModel;
        public EffectorModel HitEffectorModel => hitEffectorModel;
        public void SetHitEffectorModel(EffectorModel value) => this.hitEffectorModel = value;

        EffectorModel deathEffectorModel;
        public EffectorModel DeathEffectorModel => deathEffectorModel;
        public void SetDeathEffectorModel(EffectorModel value) => this.deathEffectorModel = value;

        #endregion

        #region [额外打击次数]

        int extraHitTimes;
        public void SetExtraHitTimes(int value) => extraHitTimes = value;
        public void ReduceHitExtraTimes() => extraHitTimes--;

        #endregion

        #region [速度组]

        float[] moveSpeedArray;
        public void SetMoveSpeedArray(float[] value) => this.moveSpeedArray = value;

        #endregion

        #region [方向组]

        Vector3[] directionArray;
        public void SetDirectionArray(Vector3[] value) => this.directionArray = value;

        #endregion

        #region [初始相对偏移]

        Vector3 relativeOffset_pos;
        public Vector3 RelativeOffset_pos => relativeOffset_pos;
        public void SetRelativeOffset_pos(Vector3 value) => this.relativeOffset_pos = value;

        Vector3Int relativeOffset_euler;
        public Vector3Int RelativeOffset_euler => relativeOffset_euler;
        public void SetRelativeOffset_euler(Vector3Int value) => this.relativeOffset_euler = value;

        #endregion

        public void Ctor() {
            // GameObject
            rootGO = new GameObject("弹道元素");
            logicRoot = new GameObject("LogicRoot");
            rendererRoot = new GameObject("VFXRoot");
            logicRoot.transform.SetParent(rootGO.transform, false);
            rendererRoot.transform.SetParent(rootGO.transform, false);

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

        public void Reset() {
            attributeCom.Reset();
            inputCom.Reset();
        }

        public void SetFather(in IDArgs father) {
            this.father = father;
        }

        public void SetVFXGO(GameObject value) {
            value.transform.SetParent(rendererRoot.transform, false);
            this.vfxGO = value;
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

        public void SetPos(Vector2 pos) {
            rootGO.transform.position = pos;
        }

        public void SetRotation(Quaternion rot) {
            rootGO.transform.rotation = rot;
        }

        public void SetRotation_UseRelativeOffset(Quaternion rot) {
            rootGO.transform.rotation = rot * Quaternion.Euler(relativeOffset_euler);
        }

        public void SetPos_UseRelativeOffset(Vector3 pos) {
            rootGO.transform.position = pos + relativeOffset_pos;
        }

        #region [根据帧 判断状态]

        public bool CanMove(int frame) {
            var index = frame - startFrame;
            if (index >= moveSpeedArray.Length) return false;
            if (index >= directionArray.Length) return false;
            return true;
        }

        public bool IsJustPassLastMoveFrame(int frame) {
            var index = frame - startFrame;
            return index == moveSpeedArray.Length - 1;
        }

        public bool TryGetFrameSpeed(int frame, out float speed) {
            var index = frame - startFrame;
            if (index < moveSpeedArray.Length) {
                speed = moveSpeedArray[index];
                return true;
            }
            speed = 0;
            return false;
        }

        public float GetFrameSpeed(int frame) {
            var index = frame - startFrame;
            return moveSpeedArray[index];
        }

        public bool TryGetFrameDirection(int frame, out Vector3 direction) {
            var index = frame - startFrame;
            if (index < directionArray.Length) {
                direction = directionArray[index];
                return true;
            }
            direction = Vector3.zero;
            return false;
        }

        public Vector3 GetFrameDirection(int frame) {
            var index = frame - startFrame;
            return directionArray[index];
        }

        public TriggerStatus GetTriggerStatus(int frame) {
            if (frame < startFrame || frame > endFrame) return TriggerStatus.None;
            if (frame == startFrame) return TriggerStatus.Begin;
            if (frame == endFrame) return TriggerStatus.End;
            return TriggerStatus.Triggering;
        }

        #endregion

        #region [表现同步]

        public void Renderer_Sync() {
            var elementPos = rootGO.transform.position;
            vfxGO.transform.position = elementPos;
        }

        public void Renderer_Easing(float dt) {
            var rendererPos = rendererRoot.transform.position;
            var logicPos = logicRoot.transform.position;
            var lerpPos = Vector3.Lerp(rendererPos, logicPos, dt * 30);// todo bug
            rendererRoot.transform.position = lerpPos;
        }

        #endregion

    }

}