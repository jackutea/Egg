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

        FootComponent footCom;
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

        public event Action<RoleEntity, Collision2D> OnCollisionEnterHandle;

        public void Ctor() {

            fsmCom = new RoleFSMComponent();
            inputRecordCom = new RoleInputRecordComponent();
            skillorSlotCom = new SkillorSlotComponent();
            weaponSlotCom = new WeaponSlotComponent();
            attrCom = new RoleAttributeComponent();

            body = transform.Find("body");
            rb = GetComponent<Rigidbody2D>();
            sr = body.Find("mesh").GetComponent<SpriteRenderer>();

            footCom = body.GetComponentInChildren<FootComponent>();

            moveCom = new MoveComponent();
            moveCom.Inject(rb);

            TDLog.Assert(rb != null);
            TDLog.Assert(footCom != null);

            footCom.OnCollisionEnterHandle += OnFootEnter;

        }

        public void TearDown() {
            footCom.OnCollisionEnterHandle -= OnFootEnter;
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
            moveCom.Move(inputRecordCom.MoveAxis, attrCom.MoveSpeed);
        }

        public void Jump() {
            moveCom.Jump(inputRecordCom.IsJump, attrCom.JumpSpeed);
        }

        public void Falling(float dt) {
            moveCom.Falling(dt, attrCom.FallingAcceleration, attrCom.FallingSpeedMax);
        }

        public void EnterGround() {
            moveCom.EnterGround();
        }

        // ==== Hit ====
        public void HitBeHurt(int atk) {
            attrCom.HitBeHurt(atk);
        }

        // ==== Phx Event ====
        void OnFootEnter(Collision2D other) {
            OnCollisionEnterHandle.Invoke(this, other);
        }

    }

}