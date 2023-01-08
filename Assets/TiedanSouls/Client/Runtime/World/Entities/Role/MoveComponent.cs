using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class MoveComponent {

        Rigidbody2D rb;

        float moveSpeed = 5.5f;
        float jumpSpeed = 14f;

        bool isJumping;
        bool isGround;

        public MoveComponent() {
            isGround = true;
        }

        public void Inject(Rigidbody2D rb) {
            this.rb = rb;
        }

        public void Move(Vector2 moveAxis) {
            var velo = rb.velocity;
            velo.x = moveAxis.x * moveSpeed;
            rb.velocity = velo;
        }

        public void Jump(bool isJumpPress) {
            if (isJumpPress && !isJumping && isGround) {
                
                isJumping = true;
                isGround = false;

                var velo = rb.velocity;
                velo.y = jumpSpeed;
                rb.velocity = velo;

            }
        }

        public void EnterGround() {
            isGround = true;
            isJumping = false;
        }
        
    }
}