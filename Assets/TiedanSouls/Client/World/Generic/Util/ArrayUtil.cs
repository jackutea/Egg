namespace TiedanSouls.Client {

    public static class ArrayUtil {

        /// <summary>
        /// 移除数组中的指定元素，其后元素左移用 ‘元素值’ 进行覆盖，剩余元素值设置为默认值
        /// </summary>
        public static void CutElementsAndLeftMove(this short[] array, int fromIndex, int toIndex, short targetValue, short defaultValue = short.MinValue) {
            int l = 0;
            int r = 1;

            while (r <= toIndex) {
                if (array[r] != targetValue) {
                    array[l] = array[r];
                    l++;
                    r++;
                } else {
                    r++;
                }
            }

            for (int i = l; i <= toIndex; i++) {
                array[i] = defaultValue;
            }
        }

        /// <summary>
        /// 移除数组中的指定元素， 其后元素左移用 ‘下标' 进行覆盖，剩余元素值设置为默认值, 返回移除元素的个数
        /// </summary>
        public static void CutElementsAndLeftMove_CoverByIndex(this short[] array, int fromIndex, int toIndex, short targetValue, short defaultValue = short.MinValue) {
            short l = 0;
            short r = 0;
            while (r <= toIndex) {
                if (array[r] != targetValue) {
                    array[l] = r;
                    l++;
                    r++;
                } else {
                    r++;
                }
            }

            for (int i = l; i <= toIndex; i++) {
                array[i] = defaultValue;
            }
        }

    }

}