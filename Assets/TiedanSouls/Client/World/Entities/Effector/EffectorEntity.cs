using System;

namespace TiedanSouls.Client.Entities {

    [Serializable]
    public class EffectorEntity {

        public int typeID;
        public string effectorName;
        public RoleEffectorModel roleEffectorModel;
        public SkillEffectorModel skillEffectorModel;

    }

}