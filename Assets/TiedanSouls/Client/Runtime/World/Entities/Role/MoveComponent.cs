using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    [Serializable]
    public class MoveComponent {

        Rigidbody2D rb;

        bool isJumping;
        bool isGround;

        public MoveComponent() {
            isJumping = false;
            isGround = false;
        }

        public void Inject(Rigidbody2D rb) {
            this.rb = rb;
        }

        public void Move(Vector2 moveAxis, float moveSpeed) {
            var velo = rb.velocity;
            velo.x = moveAxis.x * moveSpeed;
            rb.velocity = velo;
        }

        public void Jump(bool isJumpPress, float jumpSpeed) {
            if (isJumpPress && !isJumping && isGround) {

                isJumping = true;
                isGround = false;

                var velo = rb.velocity;
                velo.y = jumpSpeed;
                rb.velocity = velo;

            }
        }

        public void Falling(float dt, float fallingAcceleration, float fallingSpeedMax) {
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