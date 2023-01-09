using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    [Serializable]
    public class MoveComponent {

        Rigidbody2D rb;

        [SerializeField] float moveSpeed = 5.5f;
        [SerializeField] float jumpSpeed = 13f;
        [SerializeField] float fallingAcceleration = 24f;
        [SerializeField] float fallingSpeedMax = 40f;

        bool isJumping;
        bool isGround;

        public MoveComponent() { }

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

        public void Falling(float dt) {
            var velo = rb.velocity;
            velo.y -= fallingAcceleration * dt;
            if (velo.y < -fallingSpeedMax) {
                velo.y = -fallingSpeedMax;
            }
            rb.velocity = velo;
        }

        public void EnterGround() {
            isGround = true;
            isJumping = false;
        }

    }
}