using System.Collections.Generic;
using TiedanSouls.Generic;
using UnityEditor;
using UnityEngine;

namespace TiedanSouls.EditorTool {

    [CustomEditor(typeof(BuffEditorGO))]
    public class BuffEditorInspector : Editor {

        BuffEditorGO editorGo;

        List<SerializedProperty> propertyList;

        void OnEnable() {
            if (target == null) return;

            editorGo = target as BuffEditorGO;

            if (propertyList == null) propertyList = new List<SerializedProperty>();
            else propertyList.Clear();

            var iterator = serializedObject.GetIterator();
            iterator.NextVisible(true); // Skip the script property
            while (iterator.NextVisible(false)) {
                propertyList.Add(serializedObject.FindProperty(iterator.name));
            }
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            var labelWidth = 150f;
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            Color color = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("保存", GUILayout.Width(200), GUILayout.Height(50))) {
                editorGo.Save();
            }
            GUI.color = Color.yellow;
            if (GUILayout.Button("读取", GUILayout.Width(200), GUILayout.Height(50))) {
                editorGo.Load();
            }
            GUI.color = color;

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            var chosenRoleStateFlag = GetRoleStateFlag();

            propertyList.ForEach(p => {
                if (p.hasChildren) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(p, true);
                    EditorGUILayout.EndHorizontal();
                    return;
                }

                if (p.GetType() == typeof(RoleCtrlStatus) &&
                !chosenRoleStateFlag.Contains((RoleCtrlStatus)p.intValue)) {
                    return;
                }
                
                EditorGUILayout.PropertyField(p);
            });

            serializedObject.ApplyModifiedProperties();
        }

        RoleCtrlStatus GetRoleStateFlag() {
            var count = propertyList.Count;
            var roleStateFlag = RoleCtrlStatus.None;
            for (int i = 0; i < count; i++) {
                var p = propertyList[i];
                if (p.GetType() == typeof(RoleCtrlStatus)) {
                    roleStateFlag = (RoleCtrlStatus)p.intValue;
                    break;
                }
            }
            return roleStateFlag;
        }

    }

}