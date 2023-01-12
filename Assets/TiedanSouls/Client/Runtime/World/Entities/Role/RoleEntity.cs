using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleEntity : MonoBehaviour {

        int id;
        public int ID => id;
        public void SetID(int value) => this.id = value;

        sbyte ally;
        public sbyte Ally => ally;
        public void SetAlly(sbyte value) => this.ally = value;

        Transform body;
        Rigidbody2D rb;
        SpriteRenderer sr;

        sbyte faceXDir;
        public sbyte FaceXDir => faceXDir;

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

        HpBarHUDComponent hpBarHUDCom;
        public HpBarHUDComponent HpBarHUDCom => hpBarHUDCom;

        public event Action<RoleEntity, Collision2D> OnFootCollisionEnterHandle;
        public event Action<RoleEntity, Collider2D> OnBodyTriggerExitHandle;

        public void Ctor() {

            faceXDir = 1;

            body = transform.Find("body");

            fsmCom = new RoleFSMComponent();
            inputRecordCom = new RoleInputRecordComponent();
            skillorSlotCom = new SkillorSlotComponent();

            var weaponRoot = body.Find("weapon_root");
            weaponSlotCom = new WeaponSlotComponent();
            weaponSlotCom.Inject(weaponRoot);

            var hpBarHUDRoot = body.Find("hud_root");
            hpBarHUDCom = new HpBarHUDComponent();
            hpBarHUDCom.Inject(hpBarHUDRoot);


            attrCom = new RoleAttributeComponent();

            rb = GetComponent<Rigidbody2D>();
            sr = body.Find("mesh").GetComponent<SpriteRenderer>();

            footCom = body.Find("coll_foot").GetComponent<FootComponent>();
            footCom.Ctor();

            bodyCollCom = body.Find("coll_body").GetComponent<RoleBodyCollComponent>();
            bodyCollCom.Ctor();

            moveCom = new MoveComponent();
            moveCom.Inject(rb);

            TDLog.Assert(rb != null);
            TDLog.Assert(footCom != null);

            footCom.OnCollisionEnterHandle += OnFootCollisionEnter;
            bodyCollCom.OnBodyTriggerExitHandle += OnBodyTriggerExit;

        }

        public void TearDown() {
            footCom.OnCollisionEnterHandle -= OnFootCollisionEnter;
            GameObject.Destroy(gameObject);
        }

        // ==== Mesh ====
        public void SetMesh(Sprite spr) {
            sr.sprite = spr;
        }

        // ==== Locomotion ====
        public void SetPos(Vector2 pos) {
            transform.position = pos;
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
            moveCom.LeaveCrossPlatform();
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

        void OnBodyTriggerExit(Collider2D other) {
            OnBodyTriggerExitHandle.Invoke(this, other);
        }

    }

}