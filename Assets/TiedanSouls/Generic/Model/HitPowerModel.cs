namespace TiedanSouls.Generic {

    public struct HitPowerModel {

        public int startFrame;
        public int endFrame;

        int[] damageArray;
        public void SetDamageArray(int[] value) => damageArray = value;

        int[] hitStunFrameArray;
        public void SetHitStunFrameArray(int[] value) => hitStunFrameArray = value;

        float[] knockBackVelocityArray;
        public float[] knockBackSpeedArray => knockBackVelocityArray;
        public void SetKnockBackVelocityArray(float[] value) => knockBackVelocityArray = value;

        float[] knockUpVelocityArray;
        public float[] knockUpSpeedArray => knockUpVelocityArray;
        public void SetKnockUpVelocityArray(float[] value) => knockUpVelocityArray = value;

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