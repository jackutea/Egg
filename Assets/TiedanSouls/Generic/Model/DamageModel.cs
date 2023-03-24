namespace TiedanSouls.Generic {

    public struct DamageModel {

        public DamageType damageType;   // 伤害类型
        public int[] damageArray;      // 伤害值数组

        /// <summary>
        /// 根据帧获取伤害值
        /// </summary>
        public int GetDamage(int frame) {
            if (frame < 0) return 0;
            if (damageArray == null) return 0;
            var len = damageArray.Length;
            if (len == 0) return 0;

            return frame >= len ? damageArray[len - 1] : damageArray[frame];
        }
    }

}