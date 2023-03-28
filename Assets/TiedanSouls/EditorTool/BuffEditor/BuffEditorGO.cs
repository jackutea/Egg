using UnityEngine;
using TiedanSouls.Template;

namespace TiedanSouls.EditorTool {

    public class BuffEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public BuffSO so;

        [Header("类型ID")] public int typeID;
        [Header("名称")] public string buffName;
        [Header("描述")] public string des;
        [Header("图标")] public Sprite icon;

        [Header("延时(帧)")] public int delayFrame;
        [Header("间隔(帧)")] public int intervalFrame;
        [Header("持续时间(帧)")] public int durationFrame;
        [Header("次数")] public int triggerCount;

        [Header("属性影响(组)")] public AttributeEffectEM[] attributeEffectEMArray;

        public void Save() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }
        }

        public void Load() {
            if (so == null) {
                Debug.LogWarning("配置文件为空!");
                return;
            }
        }

    }

}