using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class WeaponAttributeComponent {

        public WeaponAttributeComponent() { }

        public void Reset() {
            TDLog.Error("武器属性重置未实现");
         }

        #region [伤害加成]

        float physicalDamage;
        public float PhysicalDamage => physicalDamage;

        float physicalDamageBase;
        public float PhysicalDamageBase => physicalDamageBase;

        float magicDamage;
        public float MagicDamage => magicDamage;

        float magicDamageBase;
        public float MagicDamageBase => magicDamageBase;

        public void SetPhysicalDamage(float physicalDamage) {
            if (physicalDamage < 0) {
                TDLog.Warning("PhysicalDamage < 0");
                return;
            }
            this.physicalDamage = physicalDamage;
        }

        public void SetMagicDamage(float magicDamage) {
            if (magicDamage < 0) {
                TDLog.Warning("MagicDamage < 0");
                return;
            }
            this.magicDamage = magicDamage;
        }

        public void AddPhysicalDamage(float physicalDamage) {
            if (physicalDamage < 0) {
                TDLog.Warning("PhysicalDamage < 0");
                return;
            }
            this.physicalDamage += physicalDamage;
        }

        public void AddMagicDamage(float magicDamage) {
            if (magicDamage < 0) {
                TDLog.Warning("MagicDamage < 0");
                return;
            }
            this.magicDamage += magicDamage;
        }

        public void ReducePhysicalDamage(float physicalDamage) {
            if (physicalDamage < 0) {
                TDLog.Warning("PhysicalDamage < 0");
                return;
            }
            this.physicalDamage -= physicalDamage;
        }

        public void ReduceMagicDamage(float magicDamage) {
            if (magicDamage < 0) {
                TDLog.Warning("MagicDamage < 0");
                return;
            }
            this.magicDamage -= magicDamage;
        }

        public void SetPhysicalDamageBase(float physicalDamageBase) {
            if (physicalDamageBase < 0) {
                TDLog.Warning("PhysicalDamageBase < 0");
                return;
            }
            this.physicalDamageBase = physicalDamageBase;
        }

        #endregion

        #region [伤害加成]

        float physicalDamageIncrease;
        public float PhysicalDamageIncrease => physicalDamageIncrease;

        float magicDamageIncrease;
        public float MagicDamageIncrease => magicDamageIncrease;

        public void AddPhysicalDamageIncrease(float physicalDamageIncrease) {
            if (physicalDamageIncrease < 0) {
                TDLog.Warning("PhysicalDamageIncrease < 0");
                return;
            }
            this.physicalDamageIncrease += physicalDamageIncrease;
        }

        public void AddMagicDamageIncrease(float magicDamageIncrease) {
            if (magicDamageIncrease < 0) {
                TDLog.Warning("MagicDamageIncrease < 0");
                return;
            }
            this.magicDamageIncrease += magicDamageIncrease;
        }

        public void ReducePhysicalDamageIncrease(float physicalDamageIncrease) {
            if (physicalDamageIncrease < 0) {
                TDLog.Warning("PhysicalDamageIncrease < 0");
                return;
            }
            this.physicalDamageIncrease -= physicalDamageIncrease;
        }

        public void ReduceMagicDamageIncrease(float magicDamageIncrease) {
            if (magicDamageIncrease < 0) {
                TDLog.Warning("MagicDamageIncrease < 0");
                return;
            }
            this.magicDamageIncrease -= magicDamageIncrease;
        }

        #endregion

        #region [防御加成]

        float physicsDefenseIncrease;
        public float PhysicsDefenseIncrease => physicsDefenseIncrease;

        float magicDefenseIncrease;
        public float MagicDefenseIncrease => magicDefenseIncrease;

        public void SetPhysicsDefenseIncrease(float physicsDefenseIncrease) {
            if (physicsDefenseIncrease < 0) {
                TDLog.Warning("PhysicsDefenseIncrease < 0");
                return;
            }
            this.physicsDefenseIncrease = physicsDefenseIncrease;
        }

        public void SetMagicDefenseIncrease(float magicDefenseIncrease) {
            if (magicDefenseIncrease < 0) {
                TDLog.Warning("MagicDefenseIncrease < 0");
                return;
            }
            this.magicDefenseIncrease = magicDefenseIncrease;
        }

        public void AddPhysicsDefenseIncrease(float physicsDefenseIncrease) {
            if (physicsDefenseIncrease < 0) {
                TDLog.Warning("PhysicsDefenseIncrease < 0");
                return;
            }
            this.physicsDefenseIncrease += physicsDefenseIncrease;
        }

        public void AddMagicDefenseIncrease(float magicDefenseIncrease) {
            if (magicDefenseIncrease < 0) {
                TDLog.Warning("MagicDefenseIncrease < 0");
                return;
            }
            this.magicDefenseIncrease += magicDefenseIncrease;
        }

        public void ReducePhysicsDefenseIncrease(float physicsDefenseIncrease) {
            if (physicsDefenseIncrease < 0) {
                TDLog.Warning("PhysicsDefenseIncrease < 0");
                return;
            }
            this.physicsDefenseIncrease -= physicsDefenseIncrease;
        }

        public void ReduceMagicDefenseIncrease(float magicDefenseIncrease) {
            if (magicDefenseIncrease < 0) {
                TDLog.Warning("MagicDefenseIncrease < 0");
                return;
            }
            this.magicDefenseIncrease -= magicDefenseIncrease;
        }

        #endregion

    }

    public static class WeaponAttributeComponentExtension {

        public static bool IsMatch(this WeaponAttributeComponent attributeCom, in WeaponAttributeSelectorModel selectorModel) {
            TDLog.Error("WeaponAttributeComponentExtension.IsMatch() not implemented");
            return false;
        }

    }

}