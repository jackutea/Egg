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
        public void SetActive(bool active) => this.isActive = active;

        public void Inject(Transform weaponRoot) {
            this.weaponRoot = weaponRoot;
        }

        public void Reset() { }

        public void SetWeapon(WeaponEntity weapon) {
            this.weapon = weapon;
            this.isActive = true;
        }

        public bool HasWeapon() {
            return weapon != null;
        }

        public void HideWeapon() {
            weaponRoot.gameObject.SetActive(false);
        }

        public void ShowWeapon() {
            weaponRoot.gameObject.SetActive(true);
        }

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