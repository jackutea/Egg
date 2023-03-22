namespace TiedanSouls.Generic {

    public struct DamageModel {

        public DamageType damageType;   // 伤害类型
        public int[] damageArray;      // 伤害值数组

        public int GetDamage(int frame) {
            return damageArray[frame];
        }
    }

}