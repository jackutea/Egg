using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public class BuffEntity : IEntity {

        public IDComponent IDCom { get; private set; }

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

        int triggerTimes;
        public int TriggerTimes => triggerTimes;
        public void SetTriggerTimes(int value) => triggerTimes = value;

        AttributeEffectModel attributeEffectModel;
        public AttributeEffectModel AttributeEffectModel => attributeEffectModel;
        public void SetAttributeEffectModel(in AttributeEffectModel value) => attributeEffectModel = value;

        int effectorTypeID;
        public int EffectorTypeID => effectorTypeID;
        public void SetEffectorTypeID(int value) => effectorTypeID = value;

        public void Ctor() {
            IDCom = new IDComponent();
            IDCom.SetEntityType(EntityType.Buff);
        }

        public void TearDown() {
        }

    }

}