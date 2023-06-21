using UnityEngine;

namespace TiedanSouls.Template {

    [CreateAssetMenu(fileName = "so_item_", menuName = "TiedanSouls/Template/物件模板", order = 0)]
    public class ItemSO : ScriptableObject {

        public ItemTM tm;

    }

}