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

        [Header("延时(帧)")] public int delayFrame;
        [Header("间隔(帧)")] public int intervalFrame;
        [Header("持续时间(帧)")] public int durationFrame;
        [Header("次数")] public int triggerTimes;

        [Header("属性影响(组)")] public AttributeEffectEM[] attributeEffectEMArray;

        public void Save() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }

            var tm = EM2TMUtil.GetTM_Buff(this);
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

            var tm = so.tm;
            this.typeID = tm.typeID;
            this.buffName = tm.buffName;
            this.description = tm.description;
            this.icon = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(tm.iconGUID));

            this.delayFrame = tm.delayFrame;
            this.intervalFrame = tm.intervalFrame;
            this.durationFrame = tm.durationFrame;
            this.triggerTimes = tm.triggerTimes;

            this.attributeEffectEMArray = TM2EMUtil.GetEMArray_AttributeEffect(tm.attributeEffectTMArray);

            EditorUtility.SetDirty(so);
            EditorUtility.SetDirty(gameObject);
            
            AssetDatabase.Refresh();
        }

    }

}