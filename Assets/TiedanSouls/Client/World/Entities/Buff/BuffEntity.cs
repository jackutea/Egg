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

        RoleAttributeEffectModel attributeEffectModel;
        public RoleAttributeEffectModel AttributeEffectModel => attributeEffectModel;
        public void SetAttributeEffectModel(RoleAttributeEffectModel value) => attributeEffectModel = value;

        int effectorTypeID;
        public int EffectorTypeID => effectorTypeID;
        public void SetEffectorTypeID(int value) => effectorTypeID = value;

        int maxExtraStackCount;
        public int MaxExtraStackCount => maxExtraStackCount;
        public void SetMaxExtraStackCount(int value) => maxExtraStackCount = value;

        int curFrame;
        public int CurFrame => curFrame;
        public void AddCurFrame() => curFrame++;
        public void ResetCurFrame() => curFrame = -1;

        int triggerTimes;
        public int TriggerTimes => triggerTimes;
        public void AddTriggerTimes() => triggerTimes++;
        public void ResetTriggerTimes() => triggerTimes = 0;

        int extraStackCount;
        public int ExtraStackCount => extraStackCount;
        public void AddExtraStackCount() => extraStackCount++;
        public void ResetExtraStackCount() => extraStackCount = 0;

        bool needRevoke;
        public bool NeedRevoke => needRevoke;
        public void SetNeedRevoke(bool value) => needRevoke = value;

        public void Ctor() {
            IDCom = new EntityIDComponent();
            IDCom.SetEntityType(EntityType.Buff);
            curFrame = -1;
        }

        public void ResetAll() {
            curFrame = -1;
            triggerTimes = 0;
            extraStackCount = 0;
            attributeEffectModel.ResetOffset();
        }

        public void SetAttributeEffectModel(in RoleAttributeEffectModel value) {
            this.attributeEffectModel = value;
        }

        public void SetFather(in EntityIDArgs father) {
            IDCom.SetFather(father);
        }

        public bool IsFinished() {
            return curFrame >= totalFrame;
        }

        public bool IsTriggerFrame() {
            return curFrame >= delayFrame && (curFrame - delayFrame) % (intervalFrame + 1) == 0;
        }

    }

}