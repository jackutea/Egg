using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 子弹
    /// </summary>
    public class BulletEntity : IEntity {

        #region [组件]

        IDComponent idCom;
        public IDComponent IDCom => idCom;

        InputComponent inputCom;
        public InputComponent InputCom => inputCom;

        AttributeComponent attributeCom;
        public AttributeComponent AttributeCom => attributeCom;

        MoveComponent moveCom;
        public MoveComponent MoveCom => moveCom;

        BulletFSMComponent fsmCom;
        public BulletFSMComponent FSMCom => fsmCom;

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

        int totalFrame;
        public int TotalFrame => totalFrame;
        public void SetTotalFrame(int value) => totalFrame = value;

        #endregion

        #region [碰撞器]

        CollisionTriggerModel collisionTriggerModel;
        public CollisionTriggerModel CollisionTriggerModel => collisionTriggerModel;
        public void SetCollisionTriggerModel(in CollisionTriggerModel value) {
            var colliderModelArray = value.colliderModelArray;
            var len = colliderModelArray.Length;
            for (int i = 0; i < len; i++) {
                var colliderModel = colliderModelArray[i];
                colliderModel.transform.SetParent(logicRoot.transform, false);
            }
            this.collisionTriggerModel = value;
        }

        #endregion

        #region [效果器]

        EffectorModel hitEffectorModel;
        public EffectorModel HitEffectorModel => hitEffectorModel;
        public void SetHitEffectorModel(EffectorModel value) => this.hitEffectorModel = value;

        EffectorModel deathEffectorModel;
        public EffectorModel DeathEffectorModel => deathEffectorModel;
        public void SetDeathEffectorModel(EffectorModel value) => this.deathEffectorModel = value;

        #endregion

        #region [速度矢量组]

        float[] moveSpeedArray;
        public void SetMoveSpeedArray(float[] value) => this.moveSpeedArray = value;

        Vector3[] moveDirArray;
        public void SetMoveDirArray(Vector3[] value) => this.moveDirArray = value;

        #endregion

        public void Ctor() {
            rootGO = new GameObject("子弹");
            logicRoot = new GameObject("Logic_Root");
            rendererRoot = new GameObject("Renderer_Root");

            logicRoot.transform.SetParent(rootGO.transform, false);
            rendererRoot.transform.SetParent(rootGO.transform, false);

            rb = logicRoot.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;

            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Bullet);

            inputCom = new InputComponent();

            moveCom = new MoveComponent();
            moveCom.Inject(rb);

            attributeCom = new AttributeComponent();

            fsmCom = new BulletFSMComponent();
        }

        public void TearDown() {
            rootGO.SetActive(false);
            vfxGO.gameObject.SetActive(false);
        }

        public void Reset() {
            attributeCom.Reset();
            inputCom.Reset();
        }

        public void SetVFXGO(GameObject value) {
            value.transform.SetParent(rendererRoot.transform, false);
            this.vfxGO = value;
        }

        public void Activate() {
            rootGO.SetActive(true);
            vfxGO.SetActive(true);
        }

        public void Deactivate() {
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

        public void SetRot(Quaternion rot) {
            rootGO.transform.rotation = rot;
            var len = moveSpeedArray.Length;
            for (int i = 0; i < len; i++) {
                var moveDir = moveDirArray[i];
                moveDirArray[i] = rot * moveDir;
            }
        }

        public bool CanMove(int frame) {
            var index = frame;
            if (index >= moveSpeedArray.Length) return false;
            if (index >= moveDirArray.Length) return false;
            return true;
        }

        public bool IsJustPassLastMoveFrame(int frame) {
            var index = frame;
            return index == moveSpeedArray.Length - 1;
        }

        public bool TryGetMoveSpeed(int frame, out float speed) {
            speed = 0;
            if (moveSpeedArray == null) {
                TDLog.Warning("子弹移动 速度 数组为空");
                return false;
            }

            var index = frame;
            if (index < moveSpeedArray.Length) {
                speed = moveSpeedArray[index];
                return true;
            }
            speed = 0;
            return false;
        }

        public bool TryGetMoveDir(int frame, out Vector3 moveDir) {
            moveDir = Vector3.zero;
            if (moveDirArray == null) {
                TDLog.Warning("子弹移动 方向 数组为空");
                return false;
            }

            var index = frame;
            if (index < moveDirArray.Length) {
                moveDir = moveDirArray[index];
                return true;
            }
            moveDir = Vector3.zero;
            return false;
        }

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

    }

}