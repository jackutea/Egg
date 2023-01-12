namespace TiedanSouls.World.Entities {

    public class HitPowerModel {

        // ==== Attack ====
        public float knockbackForce;
        public short knockbackFrame;

        public float blowUpForce;

        public short hitStunFrame;

        public short breakPowerLevel;

        // ==== Defense ====
        public short sufferPowerLevel;

        // ==== Other ====
        public short hitStopFrame;

        public HitPowerModel() { }

        public void FromTM(Template.HitPowerTM tm) {

            this.knockbackForce = tm.knockbackForce;
            this.knockbackFrame = tm.knockbackFrame;
            this.blowUpForce = tm.blowUpForce;
            this.hitStunFrame = tm.hitStunFrame;
            this.breakPowerLevel = tm.breakPowerLevel;

            this.sufferPowerLevel = tm.sufferPowerLevel;

            this.hitStopFrame = tm.hitStopFrame;
            
        }

    }

}