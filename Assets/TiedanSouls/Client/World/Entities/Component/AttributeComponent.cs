using System;
using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

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

        public void ClearHP() {
            hp = 0;
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

        public int DecreaseHP(int damage) {
            int realDamage = 0;
            var newHP = hp - damage;
            if (newHP < 0) {
                realDamage = hp;
                newHP = 0;
            } else {
                realDamage = damage;
            }
            
            hp = newHP;

            return realDamage;
        }

        public bool IsDead() {
            return hp <= 0;
        }

    }

    public static class AttributeComponentExtension {

        public static bool IsMatch(this AttributeComponent attributeCom, in AttributeSelectorModel selectorModel) {
            var hp = selectorModel.hp;
            var hp_ComparisonType = selectorModel.hp_ComparisonType;
            var hpMax = selectorModel.hpMax;
            var hpMax_ComparisonType = selectorModel.hpMax_ComparisonType;
            var ep = selectorModel.ep;
            var ep_ComparisonType = selectorModel.ep_ComparisonType;
            var epMax = selectorModel.epMax;
            var epMax_ComparisonType = selectorModel.epMax_ComparisonType;
            var gp = selectorModel.gp;
            var gp_ComparisonType = selectorModel.gp_ComparisonType;
            var gpMax = selectorModel.gpMax;
            var gpMax_ComparisonType = selectorModel.gpMax_ComparisonType;
            var moveSpeed = selectorModel.moveSpeed;
            var moveSpeed_ComparisonType = selectorModel.moveSpeed_ComparisonType;
            var jumpSpeed = selectorModel.jumpSpeed;
            var jumpSpeed_ComparisonType = selectorModel.jumpSpeed_ComparisonType;
            var fallingAcceleration = selectorModel.fallingAcceleration;
            var fallingAcceleration_ComparisonType = selectorModel.fallingAcceleration_ComparisonType;
            var fallingSpeedMax = selectorModel.fallingSpeedMax;
            var fallingSpeedMax_ComparisonType = selectorModel.fallingSpeedMax_ComparisonType;
            return hp_ComparisonType.IsMatch(attributeCom.HP, hp)
            && hpMax_ComparisonType.IsMatch(attributeCom.HPMax, hpMax)
            && ep_ComparisonType.IsMatch(attributeCom.EP, ep)
            && epMax_ComparisonType.IsMatch(attributeCom.EPMax, epMax)
            && gp_ComparisonType.IsMatch(attributeCom.GP, gp)
            && gpMax_ComparisonType.IsMatch(attributeCom.GPMax, gpMax)
            && moveSpeed_ComparisonType.IsMatch(attributeCom.MoveSpeed, moveSpeed)
            && jumpSpeed_ComparisonType.IsMatch(attributeCom.JumpSpeed, jumpSpeed)
            && fallingAcceleration_ComparisonType.IsMatch(attributeCom.FallingAcceleration, fallingAcceleration)
            && fallingSpeedMax_ComparisonType.IsMatch(attributeCom.FallingSpeedMax, fallingSpeedMax);
        }

    }

}