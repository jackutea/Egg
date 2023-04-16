using System;

namespace TiedanSouls.Client.Entities {

    [Serializable]
    public struct RoleEffectorModel {

        public int typeID;
        public string effectorName;
        public RoleAttributeSelectorModel roleAttributeSelectorModel;
        public RoleAttributeEffectModel roleAttributeEffectModel;

    }

}