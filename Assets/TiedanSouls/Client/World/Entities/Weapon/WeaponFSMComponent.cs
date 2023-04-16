using UnityEngine;
using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class WeaponFSMComponent {

        WeaponFSMState state;
        public WeaponFSMState State => state;

        public WeaponFSMComponent() {
        }

        public void Reset() {
        }

    }
}