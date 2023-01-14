using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleEntity : MonoBehaviour {

        public EntityType EntityType => EntityType.Role;

        int id;
        public int ID => id;
        public void SetID(int value) => this.id = value;

        sbyte ally;
        public sbyte Ally => ally;
        public void SetAlly(sbyte value) => this.ally = value;

        Transform body;
        public Transform Body => body;

        Rigidbody2D rb;

        sbyte faceXDir;
        public sbyte FaceXDir => faceXDir;

        RoleModComponent modCom;
        public RoleModComponent ModCom => modCom;
        
        FootComponent footCom;
        RoleBodyCollComponent bodyCollCom;

        [SerializeField] MoveComponent moveCom;
        public MoveComponent MoveCom => moveCom;

        RoleAttributeComponent attrCom;
        public RoleAttributeComponent AttrCom => attrCom;

        RoleFSMComponent fsmCom;
        public RoleFSMComponent FSMCom => fsmCom;

        RoleInputRecordComponent inputRecordCom;
        public RoleInputRecordComponent InputRecordCom => inputRecordCom;

        SkillorSlotComponent skillorSlotCom;
        public SkillorSlotComponent SkillorSlotCom => skillorSlotCom;

        WeaponSlotComponent weaponSlotCom;
        public WeaponSlotComponent WeaponSlotCom => weaponSlotCom;

        HUDSlotComponent hudSlotCom;
        public HUDSlotComponent HudSlotCom => hudSlotCom;

        public event Action<RoleEntity, Collision2D> OnFootCollisionEnterHandle;
        public event Action<RoleEntity, Collision2D> OnFootCollisionExitHandle;
        public event Action<RoleEntity, Collider2D> OnBodyTriggerExitHandle;

        public void Ctor() {

            faceXDir = 1;

            body = transform.Find("body");

            // - FSM
            fsmCom = new RoleFSMComponent();

            // - Input
            inputRecordCom = new RoleInputRecordComponent();

            // - Skillor
            skillorSlotCom = new SkillorSlotComponent();

            // - Weapon
            var weaponRoot = body.Find("weapon_root");
            weaponSlotCom = new WeaponSlotComponent();
            weaponSlotCom.Inject(weaponRoot);

            var hudRoot = body.Find("hud_root");
            hudSlotCom = new HUDSlotComponent();
            hudSlotCom.Inject(hudRoot);


            // - Attribute
            attrCom = new RoleAttributeComponent();

            // - Collider
            footCom = body.Find("coll_foot").GetComponent<FootComponent>();
            footCom.Ctor();

            bodyCollCom = body.Find("coll_body").GetComponent<RoleBodyCollComponent>();
            bodyCollCom.Ctor();

            // - Movement
            rb = GetComponent<Rigidbody2D>();
            moveCom = new MoveComponent();
            moveCom.Inject(rb);

            TDLog.Assert(rb != null);
            TDLog.Assert(footCom != null);

            // - Mod
            modCom = new RoleModComponent();

            // ==== Bind Event ====
            footCom.OnCollisionEnterHandle += OnFootCollisionEnter;
            footCom.OnCollisionExitHandle += OnFootCollisionExit;
            bodyCollCom.OnBodyTriggerExitHandle += OnBodyTriggerExit;

        }

        public void TearDown() {
            footCom.OnCollisionEnterHandle -= OnFootCollisionEnter;
            footCom.OnCollisionExitHandle -= OnFootCollisionExit;
            GameObject.Destroy(gameObject);
        }

        // ==== Mod ====
        public void SetMod(GameObject mod) {
            modCom.SetMod(mod);
        }

        // ==== Locomotion ====
        public void SetPos(Vector2 pos) {
            transform.position = pos;
        }

        public Vector2 GetPos() {
            return transform.position;
        }

        public void Move() {
            Vector2 moveAxis = inputRecordCom.MoveAxis;

            // Renderer
            if (moveAxis.x > 0) {
                faceXDir = 1;
                body.localScale = new Vector3(faceXDir, 1, 1);
            } else if (moveAxis.x < 0) {
                faceXDir = -1;
                body.localScale = new Vector3(faceXDir, 1, 1);
            }

            // Logic
            moveCom.Move(moveAxis, attrCom.MoveSpeed);
        }

        public void Dash(Vector2 dir, Vector2 force) {
            moveCom.Dash(dir, force);
        }

        public void Jump() {
            moveCom.Jump(inputRecordCom.IsJump, attrCom.JumpSpeed);
        }

        public void CrossDown() {
            if (inputRecordCom.MoveAxis.y < 0 && moveCom.IsStandCrossPlatform) {
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
            attrCom.HitBeHurt(atk);
        }

        // ==== Phx ====
        public void SetFootTrigger(bool isTrigger) {
            footCom.SetTrigger(isTrigger);
            bodyCollCom.SetTrigger(isTrigger);
        }

        void OnFootCollisionEnter(Collision2D other) {
            OnFootCollisionEnterHandle.Invoke(this, other);
        }

        void OnFootCollisionExit(Collision2D other) {
            OnFootCollisionExitHandle.Invoke(this, other);
        }

        void OnBodyTriggerExit(Collider2D other) {
            OnBodyTriggerExitHandle.Invoke(this, other);
        }

    }

}