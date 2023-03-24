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

        /// <summary>
        /// 设置速度
        /// </summary>
        public void SetVelocity(Vector3 velo) {
            rb.velocity = velo;
        }

        /// <summary>
        /// 仅设置水平方向的速度
        /// </summary>
        public void Set_Horizontal(Vector3 velo) {
            var v = rb.velocity;
            v.x = velo.x;
            rb.velocity = v;
        }

        /// <summary>
        /// 仅设置垂直方向的速度
        /// </summary>
        public void Set_Vertical(Vector3 velo) {
            var v = rb.velocity;
            v.y = velo.y;
            rb.velocity = v;
        }

        /// <summary>
        /// 停止所有方向的移动
        /// </summary>
        public void Stop() {
            rb.velocity = Vector3.zero;
        }

        /// <summary>
        /// 停止水平方向的速度
        /// </summary>
        public void Stop_Horizontal() {
            var velo = rb.velocity;
            velo.x = 0;
            rb.velocity = velo;
        }

        /// <summary>
        /// 停止垂直方向的速度
        /// </summary>
        public void Stop_Vertical() {
            var velo = rb.velocity;
            velo.y = 0;
            rb.velocity = velo;
        }

    }
}