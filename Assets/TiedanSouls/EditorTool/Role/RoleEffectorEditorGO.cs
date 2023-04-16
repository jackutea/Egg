using UnityEngine;
using UnityEditor;
using GameArki.AddressableHelper;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    public class RoleEffectorEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public RoleEffectorSO so;

        [Header("效果器")] public RoleEffectorEM effectorEM;

        public void Save() {
            so.tm = EM2TMUtil.GetEffectorTM(this.effectorEM);

            EditorUtility.SetDirty(so);
            EditorUtility.SetDirty(gameObject);

            var labelName = AssetLabelCollection.SO_EFFECTOR;
            AddressableHelper.SetAddressable(so, labelName, labelName);
        }

        public void Load() {
            this.effectorEM = TM2EMUtil.GetEffectorEM(so.tm);
        }

    }

}