using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class RoleAttributeComponent {

        public RoleAttributeComponent() { }

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
        public void SetHP(float hp) => this.hp = hp;

        float hpMax;
        public float HPMax => hpMax;
        public void SetHPMax(float hpMax) => this.hpMax = hpMax;

        float hpMaxBase;
        public float HPMaxBase => hpMaxBase;
        public void SetHPMaxBase(float hpMaxBase) => this.hpMaxBase = hpMaxBase;

        public void ClearHP() {
            hp = 0;
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
        public float SetGP(float gp) => this.gp = gp;

        float gpMax;
        public float GPMax => gpMax;
        public void SetGPMax(float gpMax) => this.gpMax = gpMax;

        float gpMaxBase;
        public float GPMaxBase => gpMaxBase;
        public void SetGPMaxBase(float gpMaxBase) => this.gpMaxBase = gpMaxBase;

        #endregion

        #region [MoveSpeed]

        float moveSpeed;
        public float MoveSpeed => moveSpeed;
        public void SetMoveSpeed(float moveSpeed) => this.moveSpeed = moveSpeed;

        float moveSpeedBase;
        public float MoveSpeedBase => moveSpeedBase;
        public void SetMoveSpeedBase(float moveSpeedBase) => this.moveSpeedBase = moveSpeedBase;

        #endregion

        #region [攻速加成]

        float normalSkillSpeedBonus;
        public float NormalSkillSpeedBonus => normalSkillSpeedBonus;
        public void SetNormalSkillSpeedBonus(float normalSkillSpeedBonus) => this.normalSkillSpeedBonus = normalSkillSpeedBonus;

        #endregion

        #region [伤害加成]

        float physicalDamageBonus;
        public float PhysicalDamageBonus => physicalDamageBonus;
        public void SetPhysicalDamageBonus(float physicalDamageBonus) => this.physicalDamageBonus = physicalDamageBonus;

        float magicalDamageBonus;
        public float MagicalDamageBonus => magicalDamageBonus;
        public void SetmagicalDamageBonus(float magicalDamageBonus) => this.magicalDamageBonus = magicalDamageBonus;

        #endregion

        #region [防御加成]

        float physicalDefenseBonus;
        public float PhysicalDefenseBonus => physicalDefenseBonus;
        public void SetPhysicalDefenseBonus(float physicalDefenseBonus) => this.physicalDefenseBonus = physicalDefenseBonus;

        float magicalDefenseBonus;
        public float MagicalDefenseBonus => magicalDefenseBonus;
        public void SetMagicalDefenseBonus(float magicalDefenseBonus)=>  this.magicalDefenseBonus = magicalDefenseBonus;

        #endregion

        #region [JumpSpeed]

        float jumpSpeed;
        public float JumpSpeed => jumpSpeed;
        public void SetJumpSpeed(float jumpSpeed) =>    this.jumpSpeed = jumpSpeed;

        float jumpSpeedBase;
        public float JumpSpeedBase => jumpSpeedBase;
        public void SetJumpSpeedBase(float jumpSpeedBase) =>  this.jumpSpeedBase = jumpSpeedBase;

        #endregion

        #region [FallSpeed]

        float fallSpeed;
        public float FallSpeed => fallSpeed;
        public void SetFallSpeed(float fallSpeed) =>    this.fallSpeed = fallSpeed;

        float fallSpeedMax;
        public float FallSpeedMax => fallSpeedMax;
        public void SetFallSpeedMax(float fallSpeedMax) =>    this.fallSpeedMax = fallSpeedMax;

        #endregion

    }

    public static class AttributeComponentExtension {

        public static bool IsMatch(this RoleAttributeComponent attributeCom, in RoleAttributeSelectorModel selectorModel) {
            if (!selectorModel.isEnabled) return true;

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
            return hp_ComparisonType.IsMatch(attributeCom.HP, hp)
            && hpMax_ComparisonType.IsMatch(attributeCom.HPMax, hpMax)
            && ep_ComparisonType.IsMatch(attributeCom.MP, mp)
            && mpMax_ComparisonType.IsMatch(attributeCom.MPMax, mpMax)
            && gp_ComparisonType.IsMatch(attributeCom.GP, gp)
            && gpMax_ComparisonType.IsMatch(attributeCom.GPMax, gpMax)
            && moveSpeed_ComparisonType.IsMatch(attributeCom.MoveSpeed, moveSpeed)
            && jumpSpeed_ComparisonType.IsMatch(attributeCom.JumpSpeed, jumpSpeed)
            && fallingAcceleration_ComparisonType.IsMatch(attributeCom.FallSpeed, fallingAcceleration)
            && fallingSpeedMax_ComparisonType.IsMatch(attributeCom.FallSpeedMax, fallingSpeedMax);
        }

    }

}