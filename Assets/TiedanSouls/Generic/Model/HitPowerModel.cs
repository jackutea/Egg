namespace TiedanSouls.Generic {

    public struct HitPowerModel {

        int[] damageArray;
        public void SetDamageArray(int[] value) => damageArray = value;

        int[] hitStunFrameArray;
        public void SetHitStunFrameArray(int[] value) => hitStunFrameArray = value;

        float[] knockBackVelocityArray;
        public float[] KnockBackVelocityArray => knockBackVelocityArray;
        public void SetKnockBackVelocityArray(float[] value) => knockBackVelocityArray = value;

        float[] knockUpVelocityArray;
        public float[] KnockUpVelocityArray => knockUpVelocityArray;
        public void SetKnockUpVelocityArray(float[] value) => knockUpVelocityArray = value;

        public int GetHitDamage(int frame) {
            return damageArray[frame];
        }

        public int GetHitStunFrame(int frame) {
            return hitStunFrameArray[frame];
        }

    }

}