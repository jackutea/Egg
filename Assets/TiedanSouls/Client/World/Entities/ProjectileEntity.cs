using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class ProjectileEntity : MonoBehaviour, IEntity {

        ControlType controlType;
        public ControlType ControlType => controlType;
        public void SetControlType(ControlType value) => this.controlType = value;

        #region [Component]

        IDComponent idCom;
        public IDComponent IDCom => idCom;

        InputComponent inputCom;
        public InputComponent InputCom => inputCom;

        AttributeComponent attributeCom;
        public AttributeComponent AttributeCom => attributeCom;

        [SerializeField] MoveComponent moveCom;
        public MoveComponent MoveCom => moveCom;

        // TODO: ProjectileFSMComponent fsmCom;

        // // 弹道也不是不可以有HUD，考虑中。。。。。。。。。
        // HUDSlotComponent hudSlotCom;
        // public HUDSlotComponent HudSlotCom => hudSlotCom;

        #endregion

        #region [Root]

        Transform logicRoot;
        public Transform LogicRoot => logicRoot;
        public Vector2 GetPos_LogicRoot() => logicRoot.position;

        Transform rendererRoot;
        public Transform RendererRoot => rendererRoot;
        public Vector2 GetPos_RendererRoot() => rendererRoot.position;

        Rigidbody2D rb_logicRoot;

        #endregion

        #region [Misc]

        Vector2 bornPos;
        public Vector2 BornPos => bornPos;
        public void SetBornPos(Vector2 value) => this.bornPos = value;

        sbyte faceDirX;
        public sbyte FaceDirX => faceDirX;

        int fromFieldTypeID;
        public int FromFieldTypeID => fromFieldTypeID;
        public void SetFromFieldTypeID(int value) => this.fromFieldTypeID = value;

        #endregion

        public void Ctor() {
            faceDirX = 1;

            // - Root
            logicRoot = transform.Find("logic_root");
            rendererRoot = transform.Find("renderer_root");

            rb_logicRoot = logicRoot.GetComponent<Rigidbody2D>();
            TDLog.Assert(rb_logicRoot != null);
            moveCom = new MoveComponent();
            moveCom.Inject(rb_logicRoot);

            // Component
            idCom = new IDComponent();
            idCom.SetEntityType(EntityType.Role);
            inputCom = new InputComponent();
            attributeCom = new AttributeComponent();
        }

        public void TearDown() {
            GameObject.Destroy(gameObject);
        }

        public void Reset() {
            // - Attribute
            attributeCom.Reset();
            // - Input
            inputCom.Reset();
            // - Movement
            moveCom.LeaveGround();
        }

        public void Hide() {
            logicRoot.gameObject.SetActive(false);
            rendererRoot.gameObject.SetActive(false);
            TDLog.Log($"隐藏角色: {idCom.EntityName} ");
        }

        public void Show() {
            logicRoot.gameObject.SetActive(true);
            rendererRoot.gameObject.SetActive(true);
            TDLog.Log($"显示角色: {idCom.EntityName} ");
        }

        // ==== Locomotion ====
        public void SetPos_Logic(Vector2 pos) {
            logicRoot.position = pos;
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

        public void Move() {
            Vector2 moveAxis = inputCom.MoveAxis;
            moveCom.Move(moveAxis, attributeCom.MoveSpeed);
        }

        public void FaceTo(sbyte dirX) {
            if (dirX == 0) {
                return;
            }

            this.faceDirX = dirX;
            var rot = logicRoot.localRotation;
            if (dirX > 0) {
                rot.y = 0;
            } else {
                rot.y = 180;
            }

            logicRoot.localRotation = rot;
        }

        public void Falling(float dt) {
            moveCom.Falling(dt, attributeCom.FallingAcceleration, attributeCom.FallingSpeedMax);
        }

        // ==== Hit ====
        public void Attribute_HP_Decrease(int atk) {
            TDLog.Log($"{idCom} 受到伤害 - {atk}");
            attributeCom.HP_Decrease(atk);
        }

        // ==== Drop ====
        public void DropBeHit(int damage, Vector2 rebornPos) {
            attributeCom.HP_Decrease(damage);
            SetPos_Logic(rebornPos);
            SyncRenderer();
        }

        // ==== Renderer ====
        public void SyncRenderer() {
            var logicPos = logicRoot.position;
            rendererRoot.position = logicPos;
        }

        public void LerpRenderer(float dt) {
            var lerpPos = Vector3.Lerp(rendererRoot.position, logicRoot.position, dt * 30);
            rendererRoot.position = lerpPos;
        }

    }

}