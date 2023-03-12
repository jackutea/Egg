using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleEntity : MonoBehaviour {

        IDComponent idCom;
        public IDComponent IDCom => idCom;

        ControlType controlType;
        public ControlType ControlType => controlType;
        public void SetControlType(ControlType value) => this.controlType = value;

        RoleAIStrategy aiStrategy;
        public RoleAIStrategy AIStrategy => aiStrategy;
        public void SetAIStrategy(RoleAIStrategy value) => this.aiStrategy = value;

        #region [Component]

        InputComponent inputCom;
        public InputComponent InputCom => inputCom;

        AttributeComponent attrCom;
        public AttributeComponent AttrCom => attrCom;

        RoleFSMComponent fsmCom;
        public RoleFSMComponent FSMCom => fsmCom;

        [SerializeField] MoveComponent moveCom;
        public MoveComponent MoveCom => moveCom;

        SkillSlotComponent skillSlotCom;
        public SkillSlotComponent SkillSlotCom => skillSlotCom;

        WeaponSlotComponent weaponSlotCom;
        public WeaponSlotComponent WeaponSlotCom => weaponSlotCom;

        RoleModComponent modCom;
        public RoleModComponent ModCom => modCom;

        HUDSlotComponent hudSlotCom;
        public HUDSlotComponent HudSlotCom => hudSlotCom;

        FootComponent footCom;
        BodyComponent bodyCom;

        #endregion

        #region [Root]

        Transform logicRoot;
        public Transform LogicRoot => logicRoot;
        public Vector2 GetPos_LogicRoot() => logicRoot.position;

        Transform rendererRoot;
        public Transform RendererRoot => rendererRoot;
        public Vector2 GetPos_RendererRoot() => rendererRoot.position;

        Transform weaponRoot;
        public Transform WeaponRoot => weaponRoot;
        public Vector2 GetPos_WeaponRoot() => weaponRoot.position;

        Rigidbody2D rb_logicRoot;
        CapsuleCollider2D coll_logicRoot;

        #endregion

        #region [Event]

        public event Action<RoleEntity, Collider2D> FootTriggerEnterAction;
        public event Action<RoleEntity, Collider2D> FootTriggerExit;
        public event Action<RoleEntity, Collider2D> BodyTriggerExitAction;

        #endregion

        #region [Misc]

        Vector2 bornPos;
        public Vector2 BornPos => bornPos;
        public void SetBornPos(Vector2 value) => this.bornPos = value;

        sbyte faceDirX;
        public sbyte FaceDirX => faceDirX;

        bool isBoss;
        public bool IsBoss => isBoss;
        public void SetIsBoss(bool value) => this.isBoss = value;

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

            coll_logicRoot = logicRoot.GetComponent<CapsuleCollider2D>();
            TDLog.Assert(coll_logicRoot != null);

            // - Foot
            footCom = logicRoot.Find("foot").GetComponent<FootComponent>();
            footCom.Ctor();

            // - Body
            bodyCom = logicRoot.Find("body").GetComponent<BodyComponent>();
            bodyCom.Ctor();

            // - Weapon
            weaponRoot = logicRoot.Find("weapon_root");
            weaponSlotCom = new WeaponSlotComponent();
            weaponSlotCom.Inject(weaponRoot);

            // - HUD
            var hudRoot = rendererRoot.Find("hud_root");
            TDLog.Assert(hudRoot != null);

            hudSlotCom = new HUDSlotComponent();
            hudSlotCom.Inject(hudRoot);

            // - Attribute
            attrCom = new AttributeComponent();

            // ==== Bind Event ====
            footCom.FootTriggerEnter += OnFootTriggerEnter;
            footCom.FootTriggerExit += OnFootCollisionExit;

            // Component
            idCom = new IDComponent();
            modCom = new RoleModComponent();
            fsmCom = new RoleFSMComponent();
            inputCom = new InputComponent();
            skillSlotCom = new SkillSlotComponent();
        }

        public void TearDown() {
            footCom.FootTriggerEnter -= OnFootTriggerEnter;
            footCom.FootTriggerExit -= OnFootCollisionExit;
            GameObject.Destroy(gameObject);
        }

        public void Reset() {
            // - Foot
            footCom.Reset();

            // - Body
            bodyCom.Reset();

            // - Weapon
            weaponSlotCom.Reset();

            // - HUD
            hudSlotCom.Reset();

            // - Attribute
            attrCom.Reset();

            // - FSM
            fsmCom.Reset();

            // - Input
            inputCom.Reset();

            // - Skill
            skillSlotCom.Reset();

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

        // ==== Mod ====
        public void SetMod(GameObject mod) {
            modCom.SetMod(mod);
        }

        // ==== Locomotion ====
        public void SetPos_Logic(Vector2 pos) {
            logicRoot.position = pos;
        }

        public Vector2 GetPos_Logic() {
            return logicRoot.position;
        }

        public float GetRot_Logic() {
            return logicRoot.rotation.z;
        }

        public void Move() {
            Vector2 moveAxis = inputCom.MoveAxis;
            moveCom.Move(moveAxis, attrCom.MoveSpeed);
        }

        public void SetFaceDirX(sbyte dirX) {
            if (dirX == 0) {
                return;
            }

            this.faceDirX = dirX;
            var scale = new Vector3(dirX, 1, 1);
            logicRoot.localScale = scale;

            modCom.SetLocalScale(scale);
        }

        public void Dash(Vector2 dir, Vector2 force) {
            moveCom.Dash(dir, force);
        }

        public void Jump() {
            moveCom.Jump(inputCom.HasInput_Locomotion_JumpDown, attrCom.JumpSpeed);
        }

        public void TryCrossDown() {
            if (inputCom.MoveAxis.y < 0 && moveCom.IsStandCrossPlatform) {
                LeaveCrossPlatform();
            }
        }

        public void Falling(float dt) {
            moveCom.Falling(dt, attrCom.FallingAcceleration, attrCom.FallingSpeedMax);
        }

        public void EnterGround() {
            moveCom.EnterGround();
            coll_logicRoot.isTrigger = false;
            // TDLog.Log($"--- 进入地面 {entityID} ");
        }

        public void LeaveGround() {
            moveCom.LeaveGround();
            // TDLog.Log($"--- 离开地面 {entityID} ");
        }

        public void EnterCrossPlatform() {
            moveCom.EnterCrossPlatform();
            // TDLog.Log($"--- 进入横跨平台 {entityID} ");
        }

        public void LeaveCrossPlatform() {
            moveCom.LeaveGround();
            coll_logicRoot.isTrigger = true;
            // TDLog.Log($"--- 离开横跨平台 {entityID} ");
        }

        // ==== Hit ====
        public void HitBeHit(int atk) {
            TDLog.Log($"{idCom.EntityName} 受到伤害 - {atk}");
            attrCom.HurtByAtk(atk);
            hudSlotCom.HpBarHUD.SetHpBar(attrCom.HP, attrCom.HPMax);
        }

        // ==== Drop ====
        public void DropBeHit(int damage, Vector2 rebornPos) {
            attrCom.HurtByAtk(damage);
            hudSlotCom.HpBarHUD.SetHpBar(attrCom.HP, attrCom.HPMax);
            SetPos_Logic(rebornPos);
            SyncRendererr();
        }

        // ==== Phx ====
        public void SetFootTrigger(bool isTrigger) {
            footCom.SetTrigger(isTrigger);
            bodyCom.SetTrigger(isTrigger);
        }

        void OnFootTriggerEnter(Collider2D other) {
            FootTriggerEnterAction.Invoke(this, other);
        }

        void OnFootCollisionExit(Collider2D other) {
            FootTriggerExit.Invoke(this, other);
        }

        void OnBodyTriggerExit(Collider2D other) {
            BodyTriggerExitAction.Invoke(this, other);
        }

        // ==== Renderer ====
        public void SyncRendererr() {
            var logicPos = logicRoot.position;
            rendererRoot.position = logicPos;
            weaponRoot.position = logicPos;
        }

        public void SyncRenderer(float dt) {
            var lerpPos = Vector3.Lerp(rendererRoot.position, logicRoot.position, dt * 30);
            rendererRoot.position = lerpPos;
            weaponRoot.position = lerpPos;
        }

    }

}