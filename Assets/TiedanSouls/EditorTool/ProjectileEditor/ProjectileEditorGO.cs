using UnityEngine;
using UnityEditor;
using GameArki.AddressableHelper;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    public class ProjectileEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public ProjectileSO so;

        [Header("类型ID")] public int typeID; 
        [Header("名称")] public string projectileName;
        [Header("弹道子弹(组)")] public ProjectileBulletEM[] projectileBulletEMArray;

        public void Save() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }

            so.tm = EM2TMUtil.GetTM_Projectile(this);

            EditorUtility.SetDirty(so);
            EditorUtility.SetDirty(gameObject);

            var labelName = AssetLabelCollection.SO_PROJECTILE;
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
            this.projectileBulletEMArray = TM2EMUtil.GetEMArray_ProjectileBullet(tm.projetileBulletTMArray);
        }

    }

}