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
        public Vector3 Pos => rootGO.transform.position;
        public Quaternion Rotation => rootGO.transform.rotation;
        public float AngleZ => rootGO.transform.rotation.z;

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

        int moveTotalFrame;
        public int MoveTotalFrame => moveTotalFrame;
        public void SetMoveTotalFrame(int value) => moveTotalFrame = value;

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

        int deathEffectorTypeID;
        public int DeathEffectorTypeID => deathEffectorTypeID;
        public void SetDeathEffectorTypeID(int value) => this.deathEffectorTypeID = value;

        #endregion

        #region [额外穿透次数]

        int extraPenetrateCount;
        public int ExtraPenetrateCount => extraPenetrateCount;
        public void SetExtraPenetrateCount(int value) => this.extraPenetrateCount = value;
        public void ReduceExtraPenetrateCount() => this.extraPenetrateCount--;

        #endregion

        #region [子弹轨迹]

        TrajectoryType trajectoryType;
        public TrajectoryType TrajectoryType => trajectoryType;
        public void SetTrajectoryType(TrajectoryType value) => this.trajectoryType = value;

        #region [跟踪]

        EntityTrackModel entityTrackModel;
        public EntityTrackModel EntityTrackModel => entityTrackModel;
        public void SetEntityTrackModel(in EntityTrackModel value) => this.entityTrackModel = value;

        #endregion

        #region [直线]

        float[] moveSpeedArray;
        public void SetMoveSpeedArray(float[] value) => this.moveSpeedArray = value;

        Vector3[] moveDirArray;
        public void SetMoveDirArray(Vector3[] value) => this.moveDirArray = value;

        #endregion

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
            GameObject.Destroy(rootGO);
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
        }

        public void Deactivate() {
            rootGO.SetActive(false);
        }

        public void Attribute_HP_Decrease(int atk) {
            attributeCom.DecreaseHP(atk);
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

        public bool TryGet_ValidCollisionTriggerModel(out CollisionTriggerModel triggerModel) {
            triggerModel = default;
            var model = fsmCom.ActivatedModel;
            var triggerStatus = collisionTriggerModel.GetTriggerStatus(model.curFrame);
            if (triggerStatus != TriggerStatus.None) {
                triggerModel = collisionTriggerModel;
                return true;
            }

            return false;
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