using UnityEngine;
using UnityEditor;
using GameArki.AddressableHelper;
using TiedanSouls.Template;
using TiedanSouls.Generic;

namespace TiedanSouls.EditorTool {

    public class BuffEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public BuffSO so;

        [Header("类型ID")] public int typeID;
        [Header("名称")] public string buffName;
        [Header("描述")] public string description;
        [Header("图标")] public Sprite icon;
        [Header("额外可叠加层数")] public int maxExtraStackCount;
        [Header("触发延时帧")] public int delayFrame;
        [Header("触发间隔帧")] public int intervalFrame;
        [Header("触发持续帧")] public int durationFrame;
        [Header("效果器")] public int effectorTypeID;
        [Header("属性影响")] public RoleAttributeModifyEM attributeEffectEM;

        public void Save() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }

            var tm = EM2TMUtil.GetBuffTM(this);
            so.tm = tm;

            var labelName = AssetLabelCollection.SO_BUFF;
            AddressableHelper.SetAddressable(so, labelName, labelName);

            EditorUtility.SetDirty(so);
            EditorUtility.SetDirty(gameObject);

            AssetDatabase.Refresh();
        }

        public void Load() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }

            TM2EMUtil.LoadToBuffEditorGO(this, so.tm);

            EditorUtility.SetDirty(so);
            EditorUtility.SetDirty(gameObject);

            AssetDatabase.Refresh();
        }

    }

}