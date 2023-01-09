using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class RoleInputRecordComponent {

        Vector2 moveAxis;
        public Vector2 MoveAxis => moveAxis;
        public void SetMoveAxis(Vector2 value) => this.moveAxis = value;

        bool isJumpingDown;
        public bool IsJump => isJumpingDown;
        public void SetJumping(bool value) => this.isJumpingDown = value;

        bool isMelee;
        public bool IsMelee => isMelee;
        public void SetMelee(bool value) => this.isMelee = value;

        bool isSpecMelee;
        public bool IsSpecMelee => isSpecMelee;
        public void SetSpecMelee(bool value) => this.isSpecMelee = value;

        bool isBoomMelee;
        public bool IsBoomMelee => isBoomMelee;
        public void SetBoomMelee(bool value) => this.isBoomMelee = value;

        bool isInfinity;
        public bool IsInfinity => isInfinity;
        public void SetInfinity(bool value) => this.isInfinity = value;

        bool isCrush;
        public bool IsCrush => isCrush;
        public void SetCrush(bool value) => this.isCrush = value;

        public RoleInputRecordComponent() { }

    }

}