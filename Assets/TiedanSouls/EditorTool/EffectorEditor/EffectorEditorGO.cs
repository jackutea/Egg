using UnityEngine;
using TiedanSouls.Template;

namespace TiedanSouls.EditorTool.EffectorEditor {

    public class EffectorEditorGO : MonoBehaviour {

        [Header("绑定配置文件")] public EffectorSO so;

        [Header("效果器")] public EffectorEM effectorEM;

        public void Save() {
            so.tm = EM2TMUtil.GetTM_Effector(this.effectorEM);
        }

        public void Load() {
            this.effectorEM = TM2EMUtil.GetEM_Effector(so.tm);
        }

    }

}