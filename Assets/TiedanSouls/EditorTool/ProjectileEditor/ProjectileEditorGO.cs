using UnityEngine;
using TiedanSouls.Template;

namespace TiedanSouls.EditorTool.SkillEditor {

    public class ProjectileEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public ProjectileSO so;

        [Header("类型ID")] public int typeID;
        [Header("弹道名称")] public string projectileName;

        [Header("根元素")] public ProjectileElementEM rootElement;
        [Header("叶元素")] public ProjectileElementEM[] leafElementEMArray;

        public void Save() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }
            var projectileTM = EM2TMUtil.GetTM_Projectile(this);
            so.tm = projectileTM;
        }

        public void Load() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }

            var projectileTM = so.tm;
            this.typeID = projectileTM.typeID;
            this.projectileName = projectileTM.projectileName;
        }

    }

}