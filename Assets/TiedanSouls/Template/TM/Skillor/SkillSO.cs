using UnityEngine;

namespace TiedanSouls.Template {

    [CreateAssetMenu(fileName = "so_skill_", menuName = "TiedanSouls/Template/技能模板", order = 0)]
    public class SkillSO : ScriptableObject {

        public SkillTM tm;

    }

}