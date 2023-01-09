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

        FootComponent footCom;
        [SerializeField] MoveComponent moveCom;

        RoleFSMComponent fsmCom;
        public RoleFSMComponent FSMCom => fsmCom;

        RoleInputRecordComponent inputRecordCom;
        public RoleInputRecordComponent InputRecordCom => inputRecordCom;

        SkillorSlotComponent skillorSlotCom;
        public SkillorSlotComponent SkillorSlotCom => skillorSlotCom;

        public event Action<RoleEntity, Collision2D> OnCollisionEnterHandle;

        public void Ctor() {

            fsmCom = new RoleFSMComponent();
            inputRecordCom = new RoleInputRecordComponent();
            skillorSlotCom = new SkillorSlotComponent();

            body = transform.Find("body");
            rb = GetComponent<Rigidbody2D>();

            footCom = body.GetComponentInChildren<FootComponent>();

            moveCom = new MoveComponent();
            moveCom.Inject(rb);

            TDLog.Assert(rb != null);
            TDLog.Assert(footCom != null);

            footCom.OnCollisionEnterHandle += OnFootEnter;

        }

        public void SetPos(Vector2 pos) {
            body.position = pos;
        }

        // ==== Locomotion ====
        public void Move() {
            moveCom.Move(inputRecordCom.MoveAxis);
        }

        public void Jump() {
            moveCom.Jump(inputRecordCom.IsJump);
        }

        public void Falling(float dt) {
            moveCom.Falling(dt);
        }

        public void EnterGround() {
            moveCom.EnterGround();
        }

        // ==== Phx Event ====
        void OnFootEnter(Collision2D other) {
            OnCollisionEnterHandle.Invoke(this, other);
        }

    }

}