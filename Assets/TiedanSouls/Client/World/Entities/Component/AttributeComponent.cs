using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class AttributeComponent {

        public AttributeComponent() { }

        public void Reset() {
            hp = hpMax;
            mp = mpMax;
            gp = gpMax;
        }

        public bool IsDead() {
            return hp <= 0;
        }

        #region [HP]

        float hp;
        public float HP => hp;

        float hpMax;
        public float HPMax => hpMax;

        float hpMaxBase;
        public float HPMaxBase => hpMaxBase;

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

        public void SetHPMaxBase(float hpMaxBase) {
            if (hpMaxBase < 0) {
                TDLog.Warning("HPMaxBase < 0");
                return;
            }
            this.hpMaxBase = hpMaxBase;
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

        #region [MP]

        float mp;
        public float MP => mp;

        float mpMax;
        public float MPMax => mpMax;

        float mpMaxBase;
        public float MPMaxBase => mpMaxBase;

        public void ClearMP() {
            mp = 0;
        }

        public float SetMP(float mp) {
            if (mp < 0) {
                TDLog.Warning("MP < 0");
                return 0;
            }
            if (mp > mpMax) {
                TDLog.Warning("MP > MPMax");
                return 0;
            }
            this.mp = mp;
            return mp;
        }

        public void SetMPMax(float mpMax) {
            if (mpMax < 0) {
                TDLog.Warning("MPMax < 0");
                return;
            }
            this.mpMax = mpMax;
        }

        public void SetMPMaxBase(float mpMaxBase) {
            if (mpMaxBase < 0) {
                TDLog.Warning("MPMaxBase < 0");
                return;
            }
            this.mpMaxBase = mpMaxBase;
        }

        public float AddMP(float mp) {
            if (mp < 0) {
                TDLog.Warning("MP恢复值 < 0");
                return 0;
            }
            var newMP = this.mp + mp;
            if (newMP > mpMax) {
                newMP = mpMax;
            }
            this.mp = newMP;
            return newMP;
        }

        public float ReduceMP(float mp) {
            if (mp < 0) {
                TDLog.Warning("MP消耗值 < 0");
                return 0;
            }
            float realMP = 0;
            var newMP = this.mp - mp;
            if (newMP < 0) {
                realMP = this.mp;
                newMP = 0;
            } else {
                realMP = mp;
            }

            this.mp = newMP;

            return realMP;
        }

        #endregion

        #region [GP]

        float gp;
        public float GP => gp;

        float gpMax;
        public float GPMax => gpMax;

        float gpMaxBase;
        public float GPMaxBase => gpMaxBase;

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

        public void SetGPMaxBase(float gpMaxBase) {
            if (gpMaxBase < 0) {
                TDLog.Warning("GPMaxBase < 0");
                return;
            }
            this.gpMaxBase = gpMaxBase;
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

        float moveSpeed;
        public float MoveSpeed => moveSpeed;

        float moveSpeedBase;
        public float MoveSpeedBase => moveSpeedBase;

        public void SetMoveSpeed(float moveSpeed) {
            if (moveSpeed < 0) {
                TDLog.Warning("MoveSpeed < 0");
                return;
            }
            this.moveSpeed = moveSpeed;
        }

        public void SetMoveSpeedBase(float moveSpeedBase) {
            if (moveSpeedBase < 0) {
                TDLog.Warning("MoveSpeedBase < 0");
                return;
            }
            this.moveSpeedBase = moveSpeedBase;
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

        float jumpSpeed;
        public float JumpSpeed => jumpSpeed;

        float jumpSpeedBase;
        public float JumpSpeedBase => jumpSpeedBase;

        public void SetJumpSpeed(float jumpSpeed) {
            if (jumpSpeed < 0) {
                TDLog.Warning("JumpSpeed < 0");
                return;
            }
            this.jumpSpeed = jumpSpeed;
        }

        public void SetJumpSpeedBase(float jumpSpeedBase) {
            if (jumpSpeedBase < 0) {
                TDLog.Warning("JumpSpeedBase < 0");
                return;
            }
            this.jumpSpeedBase = jumpSpeedBase;
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

        float fallSpeed;
        public float FallSpeed => fallSpeed;

        float fallSpeedMax;
        public float FallSpeedMax => fallSpeedMax;

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

        #region [伤害加成]

        float physicalDamageBonus;
        public float PhysicalDamageBonus => physicalDamageBonus;

        float magicDamageBonus;
        public float MagicDamageBonus => magicDamageBonus;

        public void AddPhysicalDamageBonus(float physicalDamageBonus) {
            if (physicalDamageBonus < 0) {
                TDLog.Warning("PhysicalDamageBonus < 0");
                return;
            }
            this.physicalDamageBonus += physicalDamageBonus;
        }

        public void AddMagicDamageBonus(float magicDamageBonus) {
            if (magicDamageBonus < 0) {
                TDLog.Warning("MagicDamageBonus < 0");
                return;
            }
            this.magicDamageBonus += magicDamageBonus;
        }

        public void ReducePhysicalDamageBonus(float physicalDamageBonus) {
            if (physicalDamageBonus < 0) {
                TDLog.Warning("PhysicalDamageBonus < 0");
                return;
            }
            this.physicalDamageBonus -= physicalDamageBonus;
        }

        public void ReduceMagicDamageBonus(float magicDamageBonus) {
            if (magicDamageBonus < 0) {
                TDLog.Warning("MagicDamageBonus < 0");
                return;
            }
            this.magicDamageBonus -= magicDamageBonus;
        }

        #endregion

        #region [伤害减免]

        float physicsDefenseBonus;
        public float PhysicsDefenseBonus => physicsDefenseBonus;

        float magicDefenseBonus;
        public float MagicDefenseBonus => magicDefenseBonus;

        public void SetPhysicsDefenseBonus(float physicsDefenseBonus) {
            if (physicsDefenseBonus < 0) {
                TDLog.Warning("PhysicsDefenseBonus < 0");
                return;
            }
            this.physicsDefenseBonus = physicsDefenseBonus;
        }

        public void SetMagicDefenseBonus(float magicDefenseBonus) {
            if (magicDefenseBonus < 0) {
                TDLog.Warning("MagicDefenseBonus < 0");
                return;
            }
            this.magicDefenseBonus = magicDefenseBonus;
        }

        public void AddPhysicsDefenseBonus(float physicsDefenseBonus) {
            if (physicsDefenseBonus < 0) {
                TDLog.Warning("PhysicsDefenseBonus < 0");
                return;
            }
            this.physicsDefenseBonus += physicsDefenseBonus;
        }

        public void AddMagicDefenseBonus(float magicDefenseBonus) {
            if (magicDefenseBonus < 0) {
                TDLog.Warning("MagicDefenseBonus < 0");
                return;
            }
            this.magicDefenseBonus += magicDefenseBonus;
        }

        public void ReducePhysicsDefenseBonus(float physicsDefenseBonus) {
            if (physicsDefenseBonus < 0) {
                TDLog.Warning("PhysicsDefenseBonus < 0");
                return;
            }
            this.physicsDefenseBonus -= physicsDefenseBonus;
        }

        public void ReduceMagicDefenseBonus(float magicDefenseBonus) {
            if (magicDefenseBonus < 0) {
                TDLog.Warning("MagicDefenseBonus < 0");
                return;
            }
            this.magicDefenseBonus -= magicDefenseBonus;
        }

        #endregion

    }

    public static class AttributeComponentExtension {

        public static bool IsMatch(this AttributeComponent roleAttributeCom, in AttributeSelectorModel selectorModel) {
            var hp = selectorModel.hp;
            var hp_ComparisonType = selectorModel.hp_ComparisonType;
            var hpMax = selectorModel.hpMax;
            var hpMax_ComparisonType = selectorModel.hpMax_ComparisonType;
            var mp = selectorModel.mp;
            var ep_ComparisonType = selectorModel.ep_ComparisonType;
            var mpMax = selectorModel.mpMax;
            var mpMax_ComparisonType = selectorModel.mpMax_ComparisonType;
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
            return hp_ComparisonType.IsMatch(roleAttributeCom.HP, hp)
            && hpMax_ComparisonType.IsMatch(roleAttributeCom.HPMax, hpMax)
            && ep_ComparisonType.IsMatch(roleAttributeCom.MP, mp)
            && mpMax_ComparisonType.IsMatch(roleAttributeCom.MPMax, mpMax)
            && gp_ComparisonType.IsMatch(roleAttributeCom.GP, gp)
            && gpMax_ComparisonType.IsMatch(roleAttributeCom.GPMax, gpMax)
            && moveSpeed_ComparisonType.IsMatch(roleAttributeCom.MoveSpeed, moveSpeed)
            && jumpSpeed_ComparisonType.IsMatch(roleAttributeCom.JumpSpeed, jumpSpeed)
            && fallingAcceleration_ComparisonType.IsMatch(roleAttributeCom.FallSpeed, fallingAcceleration)
            && fallingSpeedMax_ComparisonType.IsMatch(roleAttributeCom.FallSpeedMax, fallingSpeedMax);
        }

    }

}