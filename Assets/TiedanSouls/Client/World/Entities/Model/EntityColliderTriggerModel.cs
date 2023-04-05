using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    /// <summary>
    /// 实体碰撞器触发器模型
    /// </summary>
    public struct EntityColliderTriggerModel {

        public bool isEnabled;

        // 触发模式
        public TriggerMode triggerMode;
        // 固定间隔
        public TriggerFixedIntervalModel triggerFixedIntervalModel;
        // 自定义
        public TriggerCustomModel triggerCustomModel;

        // 作用实体类型
        public EntityType targetEntityType;
        // 作用目标
        public TargetGroupType hitTargetGroupType;

        // 模型: 伤害
        public DamageModel damageModel;
        // 模型: 击退
        public KnockBackModel knockBackModel;
        // 模型: 击飞
        public KnockUpModel knockUpModel;
        // 模型: 击中效果器
        public int hitEffectorTypeID;

        public EntityCollider[] entityColliderArray;

        public TriggerState GetTriggerState(int frame) {
            if (!isEnabled) return TriggerState.None;

            if (triggerMode == TriggerMode.Custom) {
                var triggerStateDic = triggerCustomModel.triggerStateDic;
                return triggerStateDic.TryGetValue(frame, out var triggerStatus) ? triggerStatus : TriggerState.None;
            }

            if (triggerMode == TriggerMode.FixedInterval) {
                var delayFrame = triggerFixedIntervalModel.delayFrame;
                var intervalFrame = triggerFixedIntervalModel.intervalFrame;
                var maintainFrame = triggerFixedIntervalModel.maintainFrame;
                if (frame < delayFrame) return TriggerState.None;
                if (frame == delayFrame) return TriggerState.Enter;

                var mod = (frame - delayFrame) % (intervalFrame + maintainFrame);
                if (mod == 0) return TriggerState.Enter;
                if (mod < intervalFrame) return TriggerState.Stay;
                if (mod == intervalFrame) return TriggerState.Exit;
                if (mod > intervalFrame) return TriggerState.None;
            }

            return TriggerState.None;
        }

        public void ActivateAll() {
            var len = entityColliderArray.Length;
            for (int i = 0; i < len; i++) {
                entityColliderArray[i].Activate();
            }
        }

        public void DeactivateAll() {
            var len = entityColliderArray.Length;
            for (int i = 0; i < len; i++) {
                entityColliderArray[i].Deactivate();
            }
        }

    }

}