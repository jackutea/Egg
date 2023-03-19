namespace TiedanSouls.Generic {

    public struct HitPowerModel {

        public int startFrame;
        public int endFrame;

        public int[] damageArray;
        public int[] hitStunFrameArray;
        public float[] knockBackSpeedArray;
        public float[] knockUpSpeedArray;

        public int GetHitDamage(int frame) {
            frame -= startFrame;
            if (damageArray == null) return 0;
            return damageArray[frame];
        }

        public int GetHitStunFrame(int frame) {
            frame -= startFrame;
            if (hitStunFrameArray == null) return 0;
            return hitStunFrameArray[frame];
        }

    }

}