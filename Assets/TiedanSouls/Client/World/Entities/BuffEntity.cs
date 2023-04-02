using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class BuffEntity : IEntity {

        public EntityIDComponent IDCom { get; private set; }

        string description;
        public string Description => description;
        public void SetDescription(string value) => description = value;

        string iconName;
        public string IconName => iconName;
        public void SetIconName(string value) => iconName = value;

        int delayFrame;
        public int DelayFrame => delayFrame;
        public void SetDelayFrame(int value) => delayFrame = value;

        int intervalFrame;
        public int IntervalFrame => intervalFrame;
        public void SetIntervalFrame(int value) => intervalFrame = value;

        int durationFrame;
        public int DurationFrame => durationFrame;
        public void SetDurationFrame(int value) => durationFrame = value;

        RoleAttributeEffectModel roleAttributeEffectModel;
        public RoleAttributeEffectModel RoleAttributeEffectModel => roleAttributeEffectModel;
        public void SetRoleAttributeEffectModel(in RoleAttributeEffectModel value) => roleAttributeEffectModel = value;

        WeaponAttributeEffectModel weaponAttributeEffectModel;
        public WeaponAttributeEffectModel WeaponAttributeEffectModel => weaponAttributeEffectModel;
        public void SetWeaponAttributeEffectModel(in WeaponAttributeEffectModel value) => weaponAttributeEffectModel = value;

        int effectorTypeID;
        public int EffectorTypeID => effectorTypeID;
        public void SetEffectorTypeID(int value) => effectorTypeID = value;

        public int curFrame;

        public void Ctor() {
            IDCom = new EntityIDComponent();
            IDCom.SetEntityType(EntityType.Buff);
            curFrame = -1;
        }

        public void TearDown() {
        }

        public void Reset() {
            curFrame = -1;
        }

        #region [Component Wrapper]

        public void SetFather(in EntityIDArgs father) {
            IDCom.SetFather(father);
        }

        #endregion

        public bool IsFinished() {
            return curFrame >= durationFrame;
        }

        public bool IsTriggerFrame() {
            return curFrame >= delayFrame && (curFrame - delayFrame) % (intervalFrame + 1) == 0;
        }

    }

}