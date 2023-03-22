using UnityEngine;
using UnityEditor;
using GameArki.AddressableHelper;
using TiedanSouls.Template;
using TiedanSouls.Generic;

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

            EditorUtility.SetDirty(so);
            EditorUtility.SetDirty(gameObject);

            var labelName = AssetsLabelCollection.SO_PROJECTILE;
            AddressableHelper.SetAddressable(so, labelName, labelName);
        }

        public void Load() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }

            var tm = so.tm;
            this.typeID = tm.typeID;
            this.projectileName = tm.projectileName;
            this.rootElement = TM2EMUtil.GetEM_ProjectileElement(tm.rootElementTM);
            this.leafElementEMArray = TM2EMUtil.GetEMArray_ProjectileElement(tm.leafElementTMArray);
        }

    }

}