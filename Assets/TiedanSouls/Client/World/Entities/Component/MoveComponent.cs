using System;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 基本运动组件
    /// </summary>
    public class MoveComponent {

        Rigidbody2D rb;
        public Rigidbody2D RB => rb;

        public Vector3 Velocity => rb.velocity;
        public void SetVelocity(Vector3 velo) => rb.velocity = velo;

        public MoveComponent() { }

        public void Inject(Rigidbody2D rb) {
            this.rb = rb;
        }

        public void Move(Vector3 dir, float moveSpeed) {
            var velo = rb.velocity;
            rb.velocity = dir * moveSpeed;
        }

        public void Move_Horizontal(float x, float moveSpeed) {
            var velo = rb.velocity;
            velo.x = x * moveSpeed;
            rb.velocity = velo;
        }

        public void Dash(Vector3 dir, Vector3 force) {
            var velo = rb.velocity;
            velo.x = dir.x * force.x;
            velo.y = dir.y * force.y;
            rb.velocity = velo;
        }

        public void Stop() {
            rb.velocity = Vector3.zero;
        }

        public void StopHorizontal() {
            var velo = rb.velocity;
            velo.x = 0;
            rb.velocity = velo;
        }

        public void StopVertical() {
            var velo = rb.velocity;
            velo.y = 0;
            rb.velocity = velo;
        }

    }
}