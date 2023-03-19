using UnityEngine;
using TiedanSouls.Template;

namespace TiedanSouls.EditorTool.SkillEditor {

    public class EffectorEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public EffectorSO so;

        [Header("实体生成模型(组)")] public EntitySummonEM[] entitySpawnEMArray;
        [Header("实体销毁模型(组)")] public EntityDestroyEM[] entityDestroyEMArray;

        public void Save() {
            so.tm = EM2TMUtil.GetTM_Effector(this);
        }

        public void Load() {
            TM2EMUtil.ToEffectorEditorGO(this, so.tm);
        }

    }

}