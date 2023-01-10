namespace TiedanSouls.World.Entities {

    public class WeaponSlotComponent {

        WeaponModel weapon;
        public WeaponModel Weapon => weapon;

        public WeaponSlotComponent() { }

        public void SetWeapon(WeaponModel weapon) {
            this.weapon = weapon;
        }

    }

}