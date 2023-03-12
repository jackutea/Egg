using TiedanSouls.Generic;
using TiedanSouls.Template;

namespace TiedanSouls {

    public static class TM2ModelUtil {

        public static void SetHitPowerModel(HitPowerModel model, HitPowerTM tm) {
     
        }

        public static void SetSkilloCancelModel(ref SkillCancelModel model, SkillCancelTM tm) {
            model.skillTypeID = tm.skillTypeID;
            model.startFrame = tm.startFrame;
            model.isCombo = tm.isCombo;
        }

    }

}