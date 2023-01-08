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

        bool isCrush;
        public bool IsCrush => isCrush;
        public void SetCrush(bool value) => this.isCrush = value;

        public RoleInputRecordComponent() { }

    }

}