using System;
using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class AttributeComponent {

        int hp;
        public int HP => hp;

        int hpMax;
        public int HPMax => hpMax;

        int ep;
        public int EP => ep;

        int epMax;
        public int EPMax => epMax;

        int gp;
        public int GP => gp;

        int gpMax;
        public int GPMax => gpMax;

        [SerializeField] float moveSpeed;
        public float MoveSpeed => moveSpeed;

        [SerializeField] float jumpSpeed;
        public float JumpSpeed => jumpSpeed;

        [SerializeField] float fallingAcceleration;
        public float FallingAcceleration => fallingAcceleration;

        [SerializeField] float fallingSpeedMax;
        public float FallingSpeedMax => fallingSpeedMax;

        public AttributeComponent() {
            moveSpeed = 4f;
            jumpSpeed = 15f;
            fallingAcceleration = 30f;
            fallingSpeedMax = 50f;
        }

        public void Reset() {
            hp = hpMax;
            ep = epMax;
            gp = gpMax;
        }

        public void InitializeHealth(int hp, int hpMax, int ep, int epMax, int gp, int gpMax) {
            this.hp = hp;
            this.hpMax = hpMax;
            this.ep = ep;
            this.epMax = epMax;
            this.gp = gp;
            this.gpMax = gpMax;
        }

        public void InitializeLocomotion(float moveSpeed, float jumpSpeed, float fallingAcceleration, float fallingSpeedMax) {
            this.moveSpeed = moveSpeed;
            this.jumpSpeed = jumpSpeed;
            this.fallingAcceleration = fallingAcceleration;
            this.fallingSpeedMax = fallingSpeedMax;
        }

        internal void HurtByAtk(int atk) {
            hp -= atk;
            if (hp < 0) {
                hp = 0;
            }
        }

        public bool IsDead() {
            return hp <= 0;
        }

    }

}