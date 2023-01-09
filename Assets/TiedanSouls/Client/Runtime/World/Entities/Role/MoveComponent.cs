using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    [Serializable]
    public class MoveComponent {

        Rigidbody2D rb;

        [SerializeField] float moveSpeed;
        [SerializeField] float jumpSpeed;
        [SerializeField] float fallingAcceleration;
        [SerializeField] float fallingSpeedMax;

        bool isJumping;
        bool isGround;

        public MoveComponent() {
            isJumping = false;
            isGround = false;

            moveSpeed = 4f;
            jumpSpeed = 15f;
            fallingAcceleration = 30f;
            fallingSpeedMax = 50f;
        }

        public void Initialize(float moveSpeed, float jumpSpeed, float fallingAcceleration, float fallingSpeedMax) {
            this.moveSpeed = moveSpeed;
            this.jumpSpeed = jumpSpeed;
            this.fallingAcceleration = fallingAcceleration;
            this.fallingSpeedMax = fallingSpeedMax;
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