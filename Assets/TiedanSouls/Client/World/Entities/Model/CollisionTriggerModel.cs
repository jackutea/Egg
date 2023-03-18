using TiedanSouls.Generic;

namespace TiedanSouls.Client.Entities {

    public struct CollisionTriggerModel {

        public int startFrame;
        public int endFrame;

        public int delayFrame;
        public int intervalFrame;
        public int maintainFrame;

        public ColliderModel[] colliderModelArray;
        public HitPowerModel hitPower;

        public TriggerStatus GetTriggerStatus(int frame) {
            if (intervalFrame == 0) {
                var endFrame_delayed = endFrame + delayFrame;
                if (frame < startFrame || frame > endFrame_delayed) return TriggerStatus.None;
                if (frame == startFrame) return TriggerStatus.Begin;
                if (frame == endFrame_delayed) return TriggerStatus.End;
                return TriggerStatus.Triggering;
            }

            if (frame < startFrame || frame > endFrame) return TriggerStatus.None;
            if (frame == startFrame) return TriggerStatus.Begin;
            if (frame == endFrame + delayFrame) return TriggerStatus.End;

            var mod = (frame - delayFrame) % (intervalFrame + maintainFrame);
            if (mod == 0) return TriggerStatus.Begin;
            if (mod == maintainFrame) return TriggerStatus.End;
            return TriggerStatus.Triggering;
        }

    }

}