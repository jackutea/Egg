using TiedanSouls.Generic;
using TiedanSouls.Template;

namespace TiedanSouls {

    public static class TM2ModelUtil {

        public static void SetHitPowerModel(HitPowerModel model, HitPowerTM tm) {
            model.knockbackForce = tm.knockbackForce;
            model.knockbackFrame = tm.knockbackFrame;
            model.blowUpForce = tm.blowUpForce;
            model.hitStunFrame = tm.hitStunFrame;
            model.breakPowerLevel = tm.breakPowerLevel;

            model.sufferPowerLevel = tm.sufferPowerLevel;

            model.hitStopFrame = tm.hitStopFrame;
        }

        public static void SetSkilloCancelModel(ref SkillCancelModel model, SkillCancelTM tm) {
            model.skillTypeID = tm.skillTypeID;
            model.startFrame = tm.startFrame;
            model.isCombo = tm.isCombo;
        }

    }

}