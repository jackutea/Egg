using UnityEngine;

namespace TiedanSouls.World.Entities {

    public class WeaponSlotComponent {

        Transform weaponRoot;
        public Transform WeaponRoot => weaponRoot;

        WeaponModel weapon;
        public WeaponModel Weapon => weapon;

        public WeaponSlotComponent() { }

        public void Inject(Transform weaponRoot) {
            this.weaponRoot = weaponRoot;
        }

        public void SetWeapon(WeaponModel weapon) {
            this.weapon = weapon;
        }

    }

}