using TiedanSouls.Generic;
using UnityEngine;

namespace TiedanSouls.Client.Entities {

    public class AttributeComponent {

        float hp;
        public float HP => hp;

        float hpMax;
        public float HPMax => hpMax;

        float ep;
        public float EP => ep;

        float epMax;
        public float EPMax => epMax;

        float gp;
        public float GP => gp;

        float gpMax;
        public float GPMax => gpMax;

        float moveSpeed;
        public float MoveSpeed => moveSpeed;

        float jumpSpeed;
        public float JumpSpeed => jumpSpeed;

        float fallSpeed;
        public float FallSpeed => fallSpeed;

        float fallSpeedMax;
        public float FallSpeedMax => fallSpeedMax;

        float damageBonus;
        public float DamageBonus => damageBonus;

        public AttributeComponent() { }

        public void Reset() {
            hp = hpMax;
            ep = epMax;
            gp = gpMax;
            damageBonus = 0;
        }

        #region [HP]

        public void ClearHP() {
            hp = 0;
        }

        public float SetHP(float hp) {
            if (hp < 0) {
                TDLog.Warning("HP < 0");
                return 0;
            }
            if (hp > hpMax) {
                TDLog.Warning("HP > HPMax");
                return 0;
            }
            this.hp = hp;
            return hp;
        }

        public void SetHPMax(float hpMax) {
            if (hpMax < 0) {
                TDLog.Warning("HPMax < 0");
                return;
            }
            this.hpMax = hpMax;
        }

        public float AddHP(float heal) {
            if (heal < 0) {
                TDLog.Warning("HP恢复值 < 0");
                return 0;
            }
            var newHP = hp + heal;
            if (newHP > hpMax) {
                newHP = hpMax;
            }
            hp = newHP;
            return newHP;
        }

        public float ReduceHP(float damage) {
            if (damage < 0) {
                TDLog.Warning("HP伤害值 < 0");
                return 0;
            }
            float realDamage = 0;
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

        #endregion

        #region [EP]

        public void ClearEP() {
            ep = 0;
        }

        public float SetEP(float ep) {
            if (ep < 0) {
                TDLog.Warning("EP < 0");
                return 0;
            }
            if (ep > epMax) {
                TDLog.Warning("EP > EPMax");
                return 0;
            }
            this.ep = ep;
            return ep;
        }

        public void SetEPMax(float epMax) {
            if (epMax < 0) {
                TDLog.Warning("EPMax < 0");
                return;
            }
            this.epMax = epMax;
        }

        public float AddEP(float ep) {
            if (ep < 0) {
                TDLog.Warning("EP恢复值 < 0");
                return 0;
            }
            var newEP = this.ep + ep;
            if (newEP > epMax) {
                newEP = epMax;
            }
            this.ep = newEP;
            return newEP;
        }

        public float ReduceEP(float ep) {
            if (ep < 0) {
                TDLog.Warning("EP消耗值 < 0");
                return 0;
            }
            float realEP = 0;
            var newEP = this.ep - ep;
            if (newEP < 0) {
                realEP = this.ep;
                newEP = 0;
            } else {
                realEP = ep;
            }

            this.ep = newEP;

            return realEP;
        }

        #endregion

        #region [GP]

        public void ClearGP() {
            gp = 0;
        }

        public float SetGP(float gp) {
            if (gp < 0) {
                TDLog.Warning("GP < 0");
                return 0;
            }
            if (gp > gpMax) {
                TDLog.Warning("GP > GPMax");
                return 0;
            }
            this.gp = gp;
            return gp;
        }

        public void SetGPMax(float gpMax) {
            if (gpMax < 0) {
                TDLog.Warning("GPMax < 0");
                return;
            }
            this.gpMax = gpMax;
        }

        public float AddGP(float gp) {
            if (gp < 0) {
                TDLog.Warning("GP恢复值 < 0");
                return 0;
            }
            var newGP = this.gp + gp;
            if (newGP > gpMax) {
                newGP = gpMax;
            }
            this.gp = newGP;
            return newGP;
        }

        public float ReduceGP(float gp) {
            if (gp < 0) {
                TDLog.Warning("GP消耗值 < 0");
                return 0;
            }
            float realGP = 0;
            var newGP = this.gp - gp;
            if (newGP < 0) {
                realGP = this.gp;
                newGP = 0;
            } else {
                realGP = gp;
            }

            this.gp = newGP;

            return realGP;
        }

        #endregion

        #region [MoveSpeed]

        public void SetMoveSpeed(float moveSpeed) {
            if (moveSpeed < 0) {
                TDLog.Warning("MoveSpeed < 0");
                return;
            }
            this.moveSpeed = moveSpeed;
        }

        public void AddMoveSpeed(float moveSpeed) {
            if (moveSpeed < 0) {
                TDLog.Warning("MoveSpeed < 0");
                return;
            }
            this.moveSpeed += moveSpeed;
        }

        public void ReduceMoveSpeed(float moveSpeed) {
            if (moveSpeed < 0) {
                TDLog.Warning("MoveSpeed < 0");
                return;
            }
            this.moveSpeed -= moveSpeed;
        }

        #endregion

        #region [JumpSpeed]

        public void SetJumpSpeed(float jumpSpeed) {
            if (jumpSpeed < 0) {
                TDLog.Warning("JumpSpeed < 0");
                return;
            }
            this.jumpSpeed = jumpSpeed;
        }

        public void AddJumpSpeed(float jumpSpeed) {
            if (jumpSpeed < 0) {
                TDLog.Warning("JumpSpeed < 0");
                return;
            }
            this.jumpSpeed += jumpSpeed;
        }

        public void ReduceJumpSpeed(float jumpSpeed) {
            if (jumpSpeed < 0) {
                TDLog.Warning("JumpSpeed < 0");
                return;
            }
            this.jumpSpeed -= jumpSpeed;
        }

        #endregion

        #region [FallSpeed]

        public void SetFallSpeed(float fallSpeed) {
            if (fallSpeed < 0) {
                TDLog.Warning("FallSpeed < 0");
                return;
            }
            this.fallSpeed = fallSpeed;
        }

        public void AddFallSpeed(float fallSpeed) {
            if (fallSpeed < 0) {
                TDLog.Warning("FallSpeed < 0");
                return;
            }
            this.fallSpeed += fallSpeed;
        }

        public void ReduceFallSpeed(float fallSpeed) {
            if (fallSpeed < 0) {
                TDLog.Warning("FallSpeed < 0");
                return;
            }
            this.fallSpeed -= fallSpeed;
        }

        public void SetFallSpeedMax(float fallSpeedMax) {
            if (fallSpeedMax < 0) {
                TDLog.Warning("FallSpeedMax < 0");
                return;
            }
            this.fallSpeedMax = fallSpeedMax;
        }

        #endregion

        #region [DamageBonus]

        public void SetDamageBonus(float damageBonus) {
            this.damageBonus = damageBonus;
        }

        public void AddDamageBonus(float damageBonus) {
            this.damageBonus += damageBonus;
        }

        #endregion

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
            && fallingAcceleration_ComparisonType.IsMatch(attributeCom.FallSpeed, fallingAcceleration)
            && fallingSpeedMax_ComparisonType.IsMatch(attributeCom.FallSpeedMax, fallingSpeedMax);
        }

    }

}