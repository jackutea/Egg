using System;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 基本运动组件
    /// </summary>
    public class MoveComponent {

        Rigidbody2D rb;
        public Rigidbody2D RB => rb;

        public Vector2 Velocity => rb.velocity;
        public void SetVelocity(Vector2 velo) => rb.velocity = velo;

        public MoveComponent() {}

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

        public void Stop() {
            rb.velocity = Vector2.zero;
        }

        public void StopHorizontal() {
            var velo = rb.velocity;
            velo.x = 0;
            rb.velocity = velo;
        }

        public void StopVertical() {
            var velo = rb.velocity;
            velo.y = 0;
            SetVelocity(velo);
        }

    }
}