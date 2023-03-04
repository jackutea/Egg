using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleEntity : MonoBehaviour {

        // ==== Identity ====
        public EntityType EntityType => EntityType.Role;

        int entityID;
        public int EntityD => entityID;
        public void SetEntityD(int value) => this.entityID = value;

        int typeID;
        public int TypeID => typeID;
        public void SetTypeID(int value) => this.typeID = value;

        string roleName;
        public string RoleName => roleName;
        public void SetRoleName(string roleName) => this.roleName = roleName;

        AllyType allyType;
        public AllyType AllyType => allyType;
        public void SetAlly(AllyType value) => this.allyType = value;

        // ==== Root ====
        Transform logicRoot;
        public Transform LogicRoot => logicRoot;
        public Vector2 GetPos_LogicRoot() => logicRoot.position;

        Transform rendererRoot;
        public Transform RendererRoot => rendererRoot;
        public Vector2 GetPos_RendererRoot() => rendererRoot.position;

        Transform weaponRoot;
        public Transform WeaponRoot => weaponRoot;
        public Vector2 GetPos_WeaponRoot() => weaponRoot.position;

        // ==== Input ====
        ControlType controlType;
        public ControlType ControlType => controlType;
        public void SetControlType(ControlType value) => this.controlType = value;

        InputComponent inputCom;
        public InputComponent InputCom => inputCom;

        // ==== AI ====
        object aiStrategy;
        public RoleAIStrategy AIStrategy => aiStrategy as RoleAIStrategy;
        public void SetAIStrategy(RoleAIStrategy value) => this.aiStrategy = value;

        // ==== Body Part ====
        Rigidbody2D rb_logicRoot;
        CapsuleCollider2D coll_logicRoot;
        FootComponent footCom;
        BodyCollComponent bodyCollCom;

        sbyte faceDirX;
        public sbyte FaceDirX => faceDirX;

        // ==== Component ====
        [SerializeField]
        MoveComponent moveCom;
        public MoveComponent MoveCom => moveCom;

        AttributeComponent attrCom;
        public AttributeComponent AttrCom => attrCom;

        RoleFSMComponent fsmCom;
        public RoleFSMComponent FSMCom => fsmCom;

        SkillorSlotComponent skillorSlotCom;
        public SkillorSlotComponent SkillorSlotCom => skillorSlotCom;

        WeaponSlotComponent weaponSlotCom;
        public WeaponSlotComponent WeaponSlotCom => weaponSlotCom;

        // ==== Renderer ====
        // - Mod
        RoleModComponent modCom;
        public RoleModComponent ModCom => modCom;

        // - HUD
        HUDSlotComponent hudSlotCom;
        public HUDSlotComponent HudSlotCom => hudSlotCom;

        // ==== Event ====
        public event Action<RoleEntity, Collider2D> FootTriggerEnterAction;
        public event Action<RoleEntity, Collider2D> FootTriggerExit;
        public event Action<RoleEntity, Collider2D> BodyTriggerExitAction;

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
            bodyCollCom = logicRoot.Find("body").GetComponent<BodyCollComponent>();
            bodyCollCom.Ctor();

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

            // - Mod
            modCom = new RoleModComponent();

            // - FSM
            fsmCom = new RoleFSMComponent();

            // - Input
            inputCom = new InputComponent();

            // - Skillor
            skillorSlotCom = new SkillorSlotComponent();
        }

        public void TearDown() {
            footCom.FootTriggerEnter -= OnFootTriggerEnter;
            footCom.FootTriggerExit -= OnFootCollisionExit;
            GameObject.Destroy(gameObject);
        }

        public void Hide() {
            logicRoot.gameObject.SetActive(false);
            rendererRoot.gameObject.SetActive(false);
            TDLog.Log($"隐藏角色: {roleName} ");
        }

        public void Show() {
            logicRoot.gameObject.SetActive(true);
            rendererRoot.gameObject.SetActive(true);
            TDLog.Log($"显示角色: {roleName} ");
        }

        // ==== Mod ====
        public void SetMod(GameObject mod) {
            modCom.SetMod(mod);
        }

        // ==== Locomotion ====
        public void SetRBPos(Vector2 pos) {
            rb_logicRoot.position = pos;
        }

        public Vector2 GetPos_RB() {
            return rb_logicRoot.position;
        }

        public float GetRot_RB() {
            return rb_logicRoot.rotation;
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
            var scale = new Vector3(faceDirX, 1, 1);
            logicRoot.localScale = scale;
            rendererRoot.localScale = scale;
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
            TDLog.Log($"--- 进入地面 {entityID} ");
        }

        public void LeaveGround() {
            moveCom.LeaveGround();
            TDLog.Log($"--- 离开地面 {entityID} ");
        }

        public void EnterCrossPlatform() {
            moveCom.EnterCrossPlatform();
            TDLog.Log($"--- 进入横跨平台 {entityID} ");
        }

        public void LeaveCrossPlatform() {
            moveCom.LeaveGround();
            coll_logicRoot.isTrigger = true;
            TDLog.Log($"--- 离开横跨平台 {entityID} ");
        }

        // ==== Hit ====
        public void HitBeHit(int atk) {
            TDLog.Log($"{entityID} 收到伤害 - {atk}");
            attrCom.HitBeHit(atk);
            hudSlotCom.HpBarHUD.SetHpBar(attrCom.HP, attrCom.HPMax);
        }

        // ==== Drop ====
        public void DropBeHit(int damage, Vector2 rebornPos) {
            attrCom.HitBeHit(damage);
            hudSlotCom.HpBarHUD.SetHpBar(attrCom.HP, attrCom.HPMax);
            SetRBPos(rebornPos);
            SyncRenderer();
        }

        // ==== Phx ====
        public void SetFootTrigger(bool isTrigger) {
            footCom.SetTrigger(isTrigger);
            bodyCollCom.SetTrigger(isTrigger);
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
        public void SyncRenderer() {
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