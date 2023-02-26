using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleEntity : MonoBehaviour {

        // ==== ID ====
        public EntityType EntityType => EntityType.Role;

        int id;
        public int ID => id;
        public void SetID(int value) => this.id = value;

        string roleName;
        public string RoleName => roleName;
        public void SetRoleName(string roleName) => this.roleName = roleName;

        AllyType allyType;
        public AllyType AllyType => allyType;
        public void SetAlly(AllyType value) => this.allyType = value;

        // ==== Root ====
        Transform logicRoot;
        public Transform LogicRoot => logicRoot;

        Transform rendererRoot;
        public Transform RendererRoot => rendererRoot;

        // ==== Input ====
        RoleControlType controlType;
        public RoleControlType ControlType => controlType;
        public void SetControlType(RoleControlType value) => this.controlType = value;

        RoleInputComponent inputCom;
        public RoleInputComponent InputCom => inputCom;

        object aiStrategy;
        public RoleAIStrategy AIStrategy => aiStrategy as RoleAIStrategy;
        public void SetAIStrategy(RoleAIStrategy value) => this.aiStrategy = value;

        // ==== Locomotion ====
        Rigidbody2D rb;
        FootComponent footCom;
        RoleBodyCollComponent bodyCollCom;

        sbyte faceXDir;
        public sbyte FaceXDir => faceXDir;

        MoveComponent moveCom;
        public MoveComponent MoveCom => moveCom;

        RoleAttributeComponent attrCom;
        public RoleAttributeComponent AttrCom => attrCom;

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

        public event Action<RoleEntity, Collider2D> FootTriggerEnterAction;
        public event Action<RoleEntity, Collider2D> FootTriggerExit;
        public event Action<RoleEntity, Collider2D> BodyTriggerExitAction;

        public void Ctor() {

            faceXDir = 1;

            logicRoot = transform.Find("logic_root");
            rendererRoot = transform.Find("renderer_root");

            // - Movement
            rb = logicRoot.GetComponent<Rigidbody2D>();
            TDLog.Assert(rb != null);
            moveCom = new MoveComponent();
            moveCom.Inject(rb);

            // - Foot
            footCom = logicRoot.Find("foot").GetComponent<FootComponent>();
            footCom.Ctor();

            // - Body
            bodyCollCom = logicRoot.Find("body").GetComponent<RoleBodyCollComponent>();
            bodyCollCom.Ctor();

            // - Weapon
            var weaponRoot = logicRoot.Find("weapon_root");
            weaponSlotCom = new WeaponSlotComponent();
            weaponSlotCom.Inject(weaponRoot);

            // - HUD
            var hudRoot = rendererRoot.Find("hud_root");
            TDLog.Assert(hudRoot != null);

            hudSlotCom = new HUDSlotComponent();
            hudSlotCom.Inject(hudRoot);

            // - Attribute
            attrCom = new RoleAttributeComponent();

            // ==== Bind Event ====
            footCom.FootTriggerEnter += OnFootTriggerEnter;
            footCom.FootTriggerExit += OnFootCollisionExit;
            bodyCollCom.OnBodyTriggerExitHandle += OnBodyTriggerExit;

            // - Mod
            modCom = new RoleModComponent();

            // - FSM
            fsmCom = new RoleFSMComponent();

            // - Input
            inputCom = new RoleInputComponent();

            // - Skillor
            skillorSlotCom = new SkillorSlotComponent();
        }

        public void TearDown() {
            footCom.FootTriggerEnter -= OnFootTriggerEnter;
            footCom.FootTriggerExit -= OnFootCollisionExit;
            GameObject.Destroy(gameObject);
        }

        // ==== Mod ====
        public void SetMod(GameObject mod) {
            modCom.SetMod(mod);
        }

        // ==== Locomotion ====
        public void SetRBPos(Vector2 pos) {
            rb.position = pos;
        }

        public Vector2 GetRBPos() {
            return rb.position;
        }

        public float GetRBAngle() {
            return rb.rotation;
        }

        public void Move() {
            Vector2 moveAxis = inputCom.MoveAxis;

            // Renderer
            if (moveAxis.x > 0) {
                faceXDir = 1;
                logicRoot.localScale = new Vector3(faceXDir, 1, 1);
            } else if (moveAxis.x < 0) {
                faceXDir = -1;
                logicRoot.localScale = new Vector3(faceXDir, 1, 1);
            }

            // Logic
            moveCom.Move(moveAxis, attrCom.MoveSpeed);
        }

        public void Dash(Vector2 dir, Vector2 force) {
            moveCom.Dash(dir, force);
        }

        public void Jump() {
            moveCom.Jump(inputCom.HasInput_Locomotion_JumpDown, attrCom.JumpSpeed);
        }

        public void CrossDown() {
            if (inputCom.MoveAxis.y < 0 && moveCom.IsStandCrossPlatform) {
                SetFootTrigger(true);
            }
        }

        public void Falling(float dt) {
            moveCom.Falling(dt, attrCom.FallingAcceleration, attrCom.FallingSpeedMax);
        }

        public void EnterGround() {
            moveCom.EnterGround();
        }

        public void LeaveGround() {
            moveCom.LeaveGround();
        }

        public void EnterCrossPlatform() {
            moveCom.EnterCrossPlatform();
        }

        public void LeaveCrossPlatform() {
            moveCom.LeaveGround();
            SetFootTrigger(false);
        }

        // ==== Hit ====
        public void HitBeHurt(int atk) {
            TDLog.Log($"{id} 收到伤害 - {atk}");
            attrCom.HitBeHurt(atk);
            hudSlotCom.HpBarHUD.SetHpBar(attrCom.HP, attrCom.HPMax);
        }

        // ==== Drop ====
        public void DropBeHurt(int damage, Vector2 rebornPos) {
            attrCom.HitBeHurt(damage);
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
            rendererRoot.position = logicRoot.position;
        }

        public void SyncRenderer(float dt) {
            rendererRoot.position = Vector3.Lerp(rendererRoot.position, logicRoot.position, dt * 30);
        }

    }

}