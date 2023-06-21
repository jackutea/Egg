using System;
using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleEntity : MonoBehaviour, IEntity {

        RoleAIStrategy aiStrategy;
        public RoleAIStrategy AIStrategy => aiStrategy;
        public void SetAIStrategy(RoleAIStrategy value) => this.aiStrategy = value;

        #region [Component]

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

        WeaponSlotComponent weaponSlotCom;
        public WeaponSlotComponent WeaponSlotCom => weaponSlotCom;

        SkillSlotComponent skillSlotCom;
        public SkillSlotComponent SkillSlotCom => skillSlotCom;

        BuffSlotComponent buffSlotCom;
        public BuffSlotComponent BuffSlotCom => buffSlotCom;

        RoleCtrlEffectSlotComponent ctrlEffectSlotCom;
        public RoleCtrlEffectSlotComponent CtrlEffectSlotCom => ctrlEffectSlotCom;

        RoleRendererComponent rendererCom;
        public RoleRendererComponent RendererCom => rendererCom;

        HUDSlotComponent hudSlotCom;
        public HUDSlotComponent HudSlotCom => hudSlotCom;

        #endregion

        #region [Root]

        Transform logicRoot;
        public Transform LogicRoot => logicRoot;

        public Vector3 RootPos => LogicRoot.position;
        public Quaternion RootRotation => LogicRoot.rotation;

        Rigidbody2D rb;
        public Rigidbody2D RB => rb;

        CapsuleCollider2D coll_LogicRoot;
        public CapsuleCollider2D Coll_LogicRoot => coll_LogicRoot;

        public void SetTrigger(bool isTrigger) => Coll_LogicRoot.isTrigger = isTrigger;

        #endregion

        #region [Locomotion]

        sbyte faceDirX;
        public sbyte FaceDirX => faceDirX;

        public int groundCount;

        #endregion

        #region [出生点 & 是否为Boss]

        Vector2 bornPos;
        public Vector2 BornPos => bornPos;
        public void SetBornPos(Vector2 value) => this.bornPos = value;

        bool isBoss;
        public bool IsBoss => isBoss;
        public void SetIsBoss(bool value) => this.isBoss = value;

        #endregion

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

            this.moveCom = new MoveComponent();
            this.idCom = new EntityIDComponent();
            this.idCom.SetEntityType(EntityType.Role);
            this.inputCom = new InputComponent();
            this.attributeCom = new RoleAttributeComponent();
            this.fsmCom = new RoleFSMComponent();
            this.skillSlotCom = new SkillSlotComponent();
            this.buffSlotCom = new BuffSlotComponent();
            this.ctrlEffectSlotCom = new RoleCtrlEffectSlotComponent();
            this.weaponSlotCom = new WeaponSlotComponent();
            this.rendererCom = new RoleRendererComponent();
            this.hudSlotCom = new HUDSlotComponent();

            this.rb = logicRoot.GetComponent<Rigidbody2D>();
            this.coll_LogicRoot = logicRoot.GetComponent<CapsuleCollider2D>();
            this.moveCom.Inject(RB);
            this.weaponSlotCom.Inject(weaponRoot);
            this.rendererCom.Inject(rendererRoot);
            this.hudSlotCom.Inject(hudRoot);
        }

        public void TearDown() {
            GameObject.Destroy(gameObject);
        }

        public void Reset() {
            // - Weapon
            WeaponSlotCom.Reset();
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
            hudSlotCom.Reset(AttributeCom.GP, AttributeCom.HP, AttributeCom.HPMax);
        }

        public void SetFromFieldTypeID(int fieldTypeID) {
            IDCom.SetFromFieldTypeID(fieldTypeID);

            var idArgs = IDCom.ToArgs();
            SkillSlotCom.SetFather(idArgs);
            BuffSlotCom.SetFather(idArgs);
            Coll_LogicRoot.GetComponent<EntityCollider>().SetFather(idArgs);
        }

        public void Show(){
            logicRoot.gameObject.SetActive(true);
            rendererCom.ShowRenderer();
            hudSlotCom.ShowHUD();
            weaponSlotCom.ShowWeapon();
        }

        public void Hide() {
            LogicRoot.gameObject.SetActive(false);
            rendererCom.HideRenderer();
            hudSlotCom.HideHUD();
            weaponSlotCom.HideWeapon();
        }

        public void TryMoveByInput() {
            if (!InputCom.HasMoveOpt) return;

            Vector2 moveAxis = InputCom.MoveAxis;
            MoveCom.MoveHorizontal(moveAxis.x, AttributeCom.MoveSpeed);
        }

        public bool TryJumpByInput() {
            if (!InputCom.InputJump) return false;
            if (!FSMCom.PositionStatus.Contains(RolePositionStatus.OnGround)
            && !FSMCom.PositionStatus.Contains(RolePositionStatus.OnCrossPlatform)) return false;

            var rb = MoveCom.RB;
            var velo = rb.velocity;
            var jumpSpeed = AttributeCom.JumpSpeed;
            velo.y = jumpSpeed;
            MoveCom.SetVelocity(velo);

            FSMCom.Enter_JumpingUp();

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