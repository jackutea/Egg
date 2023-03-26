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

        public Vector3 LogicPos => logicRoot.transform.position;
        public Quaternion LogicRotation => logicRoot.transform.rotation;
        public float LogicAngleZ => logicRoot.transform.rotation.z;

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

        public EntityTrackModel entityTrackModel;   // 实体跟踪模型

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

        public void Reset() {
            // Com Reset
            inputCom.Reset();
            attributeCom.Reset();
            moveCom.Reset();
            fsmCom.Reset();

            // Root Reset
            rootGO.transform.position = Vector3.zero;
            rootGO.transform.rotation = Quaternion.identity;
            logicRoot.transform.position = Vector3.zero;
            logicRoot.transform.rotation = Quaternion.identity;
            rendererRoot.transform.position = Vector3.zero;
            rendererRoot.transform.rotation = Quaternion.identity;

            extraPenetrateCount = 0;
        }

        public void SetVFXGO(GameObject value) {
            value.transform.SetParent(rendererRoot.transform, false);
            this.vfxGO = value;
        }

        public void TearDown() {
            rootGO.SetActive(false);
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

        public void SetLogicPos(Vector3 pos) {
            logicRoot.transform.position = pos;
        }

        public void SetLogicRotation(Quaternion rot) {
            logicRoot.transform.rotation = rot;
        }

        public void RotateMoveDir(Quaternion rot) {
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

        public void SyncRenderer() {
            rendererRoot.transform.position = logicRoot.transform.position;
            rendererRoot.transform.rotation = logicRoot.transform.rotation;
        }

        public void EasingRenderer(float dt) {
            var rendererPos = rendererRoot.transform.position;
            var logicPos = logicRoot.transform.position;
            var lerpPos = Vector3.Lerp(rendererPos, logicPos, dt * 30);// todo bug
            rendererRoot.transform.position = lerpPos;
        }

    }

}