using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class WeaponSlotComponent {

        Transform weaponRoot;
        public Transform WeaponRoot => weaponRoot;

        WeaponModel weapon;
        public WeaponModel Weapon => weapon;

        public WeaponSlotComponent() { }

        bool isActive;
        public bool IsActive => isActive;

        public void Inject(Transform weaponRoot) {
            this.weaponRoot = weaponRoot;
        }

        public void SetWeapon(WeaponModel weapon) {
            this.weapon = weapon;
        }

        public bool HasWeapon() {
            return weapon != null;
        }

        public void SetWeaponActive(bool active) {
            TDLog.Log($"设置武器槽激活状态: {active}");
            this.isActive = active;
        }

        public void Reset(){}

    }

}