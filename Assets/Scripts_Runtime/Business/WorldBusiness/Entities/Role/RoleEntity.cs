using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleEntity : MonoBehaviour {

        RoleAIStrategy aiStrategy;
        public RoleAIStrategy AIStrategy => aiStrategy;
        public void SetAIStrategy(RoleAIStrategy value) => this.aiStrategy = value;

        public EntityIDComponent idCom;
        public EntityIDComponent IDCom => idCom;

        InputComponent inputCom;
        public InputComponent InputCom => inputCom;

        RoleAttributeComponent attributeCom;
        public RoleAttributeComponent AttributeCom => attributeCom;

        MoveComponent moveCom;
        public MoveComponent MoveCom => moveCom;

        RoleFSMComponent fsmCom;
        public RoleFSMComponent FSMCom => fsmCom;

        SkillSlotComponent skillSlotCom;
        public SkillSlotComponent SkillSlotCom => skillSlotCom;

        BuffSlotComponent buffSlotCom;
        public BuffSlotComponent BuffSlotCom => buffSlotCom;

        RoleRendererComponent rendererCom;
        public RoleRendererComponent RendererCom => rendererCom;

        Transform logicRoot;
        public Transform LogicRoot => logicRoot;

        public Vector3 RootPos => LogicRoot.position;
        public Quaternion RootRotation => LogicRoot.rotation;

        Rigidbody2D rb;

        EntityCollider entityCollider;
        public EntityCollider EntityCollider => entityCollider;
        public void SetColliderTrigger(bool isTrigger) => entityCollider.Coll.isTrigger = isTrigger;
        public void SetColliderActive(bool isActive) => entityCollider.gameObject.SetActive(isActive);

        sbyte faceDirX;
        public sbyte FaceDirX => faceDirX;

        public int groundCount;

        bool isBoss;
        public bool IsBoss => isBoss;
        public void SetIsBoss(bool value) => this.isBoss = value;

        public void Ctor() {
            faceDirX = 1;

            this.logicRoot = transform.Find("logic_root");
            var rendererRoot = transform.Find("renderer_root");
            var weaponRoot = transform.Find("weapon_root");
            var hudRoot = transform.Find("hud_root");

            TDLog.Assert(logicRoot != null, "logicRoot == null");
            TDLog.Assert(rendererRoot != null, "rendererRoot == null");
            TDLog.Assert(weaponRoot != null, "weaponRoot == null");
            TDLog.Assert(hudRoot != null, "hudRoot == null");

            this.idCom = new EntityIDComponent();
            this.idCom.SetEntityType(EntityType.Role);
            idCom.SetHolderPtr(this);

            this.moveCom = new MoveComponent();
            this.inputCom = new InputComponent();
            this.attributeCom = new RoleAttributeComponent();
            this.fsmCom = new RoleFSMComponent();
            this.skillSlotCom = new SkillSlotComponent();
            this.buffSlotCom = new BuffSlotComponent();
            this.rendererCom = new RoleRendererComponent();

            this.rb = logicRoot.GetComponent<Rigidbody2D>();
            this.entityCollider = logicRoot.GetComponent<EntityCollider>();
            this.moveCom.Inject(rb);
            this.rendererCom.Inject(rendererRoot);
        }

        public void TearDown() {
            GameObject.Destroy(gameObject);
        }

        public void Reset() {
            // - Attribute
            AttributeCom.Reset();
            // - FSM
            FSMCom.ResetAll();
            // - Input
            InputCom.Reset();
            // - Movement
            MoveCom.Reset();
            // - Renderer
            rendererCom.Reset();
        }

        public void SetFromFieldTypeID(int fieldTypeID) {
            idCom.SetFromFieldTypeID(fieldTypeID);

            SkillSlotCom.SetFather(idCom);
            BuffSlotCom.SetFather(idCom);
            entityCollider.SetHolder(idCom);
        }

        public void Show(){
            logicRoot.gameObject.SetActive(true);
            rendererCom.ShowRenderer();
        }

        public void Hide() {
            LogicRoot.gameObject.SetActive(false);
            rendererCom.HideRenderer();
        }

        public void TryMoveByInput() {
            if (!InputCom.HasMoveOpt) return;

            Vector2 moveAxis = InputCom.MoveAxis;
            MoveCom.MoveHorizontal(moveAxis.x, AttributeCom.MoveSpeed);
        }

        public bool TryJumpByInput() {
            if (!InputCom.InputJump) return false;

            var rb = MoveCom.RB;
            var velo = rb.velocity;
            var jumpSpeed = AttributeCom.JumpSpeed;
            velo.y = jumpSpeed;
            MoveCom.SetVelocity(velo);

            return true;
        }

        public void Fall(float dt) {
            var fallSpeed = AttributeCom.FallSpeed;
            var fallSpeedMax = AttributeCom.FallSpeedMax;

            var vel = MoveCom.Velocity;
            vel.y = Mathf.Max(vel.y - fallSpeed * dt, -fallSpeedMax);
            MoveCom.SetVerticalVelocity(vel);
        }

        public void HorizontalFaceTo(float dirX) {
            if (Mathf.Abs(dirX) < 0.01f) return;

            var rot = LogicRoot.localRotation;
            bool isRight = dirX > 0;
            if (isRight) rot.y = 0;
            else rot.y = 180;
            LogicRoot.localRotation = rot;
        }

        public void Stop() {
            MoveCom.Stop();
        }

        public void SetPos(Vector2 pos) {
            logicRoot.position = pos;
        }

        public void SetRotation(Quaternion rot) {
            logicRoot.rotation = rot;
        }

        public Vector3 GetHeadPos() {
            return logicRoot.position + Vector3.up * 2f;
        }

    }

}