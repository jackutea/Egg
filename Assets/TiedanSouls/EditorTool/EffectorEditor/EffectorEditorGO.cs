using UnityEngine;
using UnityEditor;
using GameArki.AddressableHelper;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool.EffectorEditor {

    public class EffectorEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public EffectorSO so;

        [Header("效果器")] public EffectorEM effectorEM;

        public void Save() {
            so.tm = EM2TMUtil.GetTM_Effector(this.effectorEM);

            EditorUtility.SetDirty(so);
            EditorUtility.SetDirty(gameObject);

            var labelName = AssetLabelCollection.SO_EFFECTOR;
            AddressableHelper.SetAddressable(so, labelName, labelName);
        }

        public void Load() {
            this.effectorEM = TM2EMUtil.GetEM_Effector(so.tm);
        }

    }

}