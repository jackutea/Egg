using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 子弹
    /// </summary>
    public class BulletEntity {

        #region [组件]

        EntityIDComponent idCom;
        public EntityIDComponent IDCom => idCom;

        InputComponent inputCom;
        public InputComponent InputCom => inputCom;

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
        public Vector3 LogicPos => logicRoot.transform.position;
        public Quaternion LogicRotation => logicRoot.transform.rotation;
        public float LogicAngleZ => logicRoot.transform.rotation.z;
        public void SetLogicPos(Vector3 pos) => logicRoot.transform.position = pos;
        public void SetLogicRotation(Quaternion rot) => logicRoot.transform.rotation = rot;

        GameObject rendererRoot;
        public GameObject RendererRoot => rendererRoot;

        Quaternion baseRotation;
        public Quaternion BaseRotation => baseRotation;
        public void SetBaseRotation(Quaternion value) => baseRotation = value;

        #endregion

        #region [表现层]

        GameObject vfxGO;
        public GameObject VFXGO => vfxGO;

        #endregion

        #region [碰撞器]

        ColliderToggleModel collisionTriggerModel;
        public ColliderToggleModel CollisionTriggerModel => collisionTriggerModel;
        public void SetCollisionTriggerModel(in ColliderToggleModel value) {
            var colliderModelArray = value.entityColliderArray;
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

        #region [飞行轨迹]

        TrajectoryType trajectoryType;
        public TrajectoryType TrajectoryType => trajectoryType;
        public void SetTrajectoryType(TrajectoryType value) => this.trajectoryType = value;

        public MoveCurveModel moveCurveModel;       // 位移曲线模型
        public EntityTrackModel entityTrackModel;   // 实体跟踪模型

        #endregion

        int maintainFrame;
        public int MaintainFrame => maintainFrame;
        public void SetMaintainFrame(int value) => this.maintainFrame = value;

        public void Ctor() {
            rootGO = new GameObject("子弹");
            logicRoot = new GameObject("Logic_Root");
            rendererRoot = new GameObject("Renderer_Root");

            logicRoot.transform.SetParent(rootGO.transform, false);
            rendererRoot.transform.SetParent(rootGO.transform, false);

            rb = logicRoot.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;

            idCom = new EntityIDComponent();
            idCom.SetEntityType(EntityType.Bullet);

            inputCom = new InputComponent();

            moveCom = new MoveComponent();
            moveCom.Inject(rb);

            fsmCom = new BulletFSMComponent();
        }

        public void Reset() {
            // Com Reset
            inputCom.Reset();
            moveCom.Reset();
            fsmCom.Reset();

            // Root Reset
            rootGO.transform.position = Vector3.zero;
            rootGO.transform.rotation = Quaternion.identity;
            logicRoot.transform.position = Vector3.zero;
            logicRoot.transform.rotation = Quaternion.identity;
            rendererRoot.transform.position = Vector3.zero;
            rendererRoot.transform.rotation = Quaternion.identity;

            rootGO.SetActive(false);

            extraPenetrateCount = 0;
        }

        public void Activate() {
            rootGO.SetActive(true);
        }

        public void Deactivate() {
            rootGO.SetActive(false);
        }

        public void SetVFXGO(GameObject value) {
            value.transform.SetParent(rendererRoot.transform, false);
            this.vfxGO = value;
        }

        public void SetFromFieldTypeID(int fieldTypeID) {
            idCom.SetFromFieldTypeID(fieldTypeID);
        }

        #region [查]

        public bool CanMove(int frame) {
            var index = frame;
            var moveSpeedArray = moveCurveModel.moveSpeedArray;
            var moveDirArray = moveCurveModel.moveDirArray;
            if (index >= moveSpeedArray.Length) return false;
            if (index >= moveDirArray.Length) return false;
            return true;
        }

        public bool IsJustPassLastMoveFrame(int frame) {
            var index = frame;
            var moveSpeedArray = moveCurveModel.moveSpeedArray;
            return index == moveSpeedArray.Length - 1;
        }

        public bool TryGet_ValidCollisionTriggerModel(out ColliderToggleModel triggerModel) {
            triggerModel = default;
            var model = fsmCom.ActivatedModel;
            var triggerStatus = collisionTriggerModel.GetToggleState(model.curFrame);
            if (triggerStatus != ToggleState.None) {
                triggerModel = collisionTriggerModel;
                return true;
            }

            return false;
        }

        public bool TryGetMoveSpeed(int frame, out float speed) {
            speed = 0;
            var moveSpeedArray = moveCurveModel.moveSpeedArray;
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
            var moveDirArray = moveCurveModel.moveDirArray;
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

        #endregion

        public void SyncRenderer() {
            rendererRoot.transform.position = logicRoot.transform.position;
            rendererRoot.transform.rotation = logicRoot.transform.rotation;
        }

        public void EasingRenderer(float dt) {
            var rendererPos = rendererRoot.transform.position;
            var logicPos = logicRoot.transform.position;
            var lerpPos = Vector3.Lerp(rendererPos, logicPos, dt * 30);// todo bug
            var lerRot = Quaternion.Lerp(rendererRoot.transform.rotation, logicRoot.transform.rotation, dt * 30);
            rendererRoot.transform.position = lerpPos;
            rendererRoot.transform.rotation = lerRot;

        }

    }

}