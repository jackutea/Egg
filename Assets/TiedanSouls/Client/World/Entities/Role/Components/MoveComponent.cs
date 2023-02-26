using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class MoveComponent {

        Rigidbody2D rb;
        public Vector2 Velocity => rb.velocity;
        public void SetVelocity(Vector2 velo) {
            rb.velocity = velo;
        }

        bool isJumping;
        bool isGrounded;
        public bool IsGrounded => isGrounded;

        bool isStandCrossPlatform;
        public bool IsStandCrossPlatform => isStandCrossPlatform;

        public MoveComponent() {
            isJumping = false;
            isGrounded = false;
        }

        public void Inject(Rigidbody2D rb) {
            this.rb = rb;
        }

        public void Move(Vector2 moveAxis, float moveSpeed) {
            var velo = rb.velocity;
            velo.x = moveAxis.x * moveSpeed;
            SetVelocity(velo);
        }

        public void Dash(Vector2 dir, Vector2 force) {
            var velo = rb.velocity;
            velo.x = dir.x * force.x;
            velo.y = dir.y * force.y;
            SetVelocity(velo);
        }

        public void KnockBack(float dir, float force) {
            var velo = rb.velocity;
            velo.x = force * dir;
            SetVelocity(velo);
        }

        public void StopHorizontal() {
            var velo = rb.velocity;
            velo.x = 0;
            SetVelocity(velo);
        }

        public void Jump(bool isJumpPress, float jumpSpeed) {
            if (isJumpPress && !isJumping && isGrounded) {

                isJumping = true;
                isGrounded = false;

                var velo = rb.velocity;
                velo.y = jumpSpeed;
                SetVelocity(velo);

            }
        }

        public void Falling(float dt, float fallingAcceleration, float fallingSpeedMax) {
            var velo = rb.velocity;
            var offset = fallingAcceleration * dt;
            velo.y -= offset;
            if (velo.y < -fallingSpeedMax) {
                velo.y = -fallingSpeedMax;
            }
            SetVelocity(velo);
        }

        public void EnterGround() {
            isGrounded = true;
            isJumping = false;
        }

        public void LeaveGround() {
            isGrounded = false;
            isStandCrossPlatform = false;
        }

        public void EnterCrossPlatform() {
            EnterGround();
            isStandCrossPlatform = true;
        }

    }
}