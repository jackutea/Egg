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

        int totalFrame;
        public int TotalFrame => totalFrame;
        public void SetTotalFrame(int value) => totalFrame = value;

        AttributeEffectModel attributeEffectModel;
        public AttributeEffectModel AttributeEffectModel => attributeEffectModel;
        public void SetAttributeEffectModel(AttributeEffectModel value) => attributeEffectModel = value;

        int effectorTypeID;
        public int EffectorTypeID => effectorTypeID;
        public void SetEffectorTypeID(int value) => effectorTypeID = value;

        public int curFrame;
        public int triggerTimes;

        public void Ctor() {
            IDCom = new EntityIDComponent();
            IDCom.SetEntityType(EntityType.Buff);
            curFrame = -1;
            triggerTimes = 0;
        }

        public void Reset() {
            curFrame = -1;
            triggerTimes = 0;
            attributeEffectModel.ResetOffset();
        }

        public void SetAttributeEffectModel(in AttributeEffectModel value) {
            this.attributeEffectModel = value;
        }

        #region [Component Wrapper]

        public void SetFather(in EntityIDArgs father) {
            IDCom.SetFather(father);
        }

        #endregion

        public bool IsFinished() {
            return curFrame >= totalFrame;
        }

        public bool IsTriggerFrame() {
            return curFrame >= delayFrame && (curFrame - delayFrame) % (intervalFrame + 1) == 0;
        }

    }

}