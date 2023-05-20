using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class WeaponSlotComponent {

        Transform weaponRoot;
        public Transform WeaponRoot => weaponRoot;

        WeaponEntity weapon;
        public WeaponEntity Weapon => weapon;

        public WeaponSlotComponent() { }

        bool isActive;
        public bool IsActive => isActive;

        public void Inject(Transform weaponRoot) {
            this.weaponRoot = weaponRoot;
        }

        public void SetWeapon(WeaponEntity weapon) {
            this.weapon = weapon;
            this.isActive = true;
        }

        public bool HasWeapon() {
            return weapon != null;
        }

        public void SetWeaponActive(bool active) {
            TDLog.Log($"设置武器槽激活状态: {active}");
            this.isActive = active;
        }

        public void Reset() { }

        public void LerpPosition(Vector3 dstPos, float dt) {
            if (Vector3.Distance(weaponRoot.position, dstPos) < GameCollection.LERP_MIN_DISTANCE) {
                weaponRoot.position = dstPos;
                return;
            }

            var ratio = dt / GameCollection.LERP_DURATION;
            weaponRoot.position = Vector3.Lerp(weaponRoot.position, dstPos, ratio);
        }

        public void LerpRotation(Quaternion dstRot, float dt) {
            weaponRoot.rotation = dstRot;
        }


    }

}