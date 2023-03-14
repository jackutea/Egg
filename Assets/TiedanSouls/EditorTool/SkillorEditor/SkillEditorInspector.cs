using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TiedanSouls.EditorTool.SkillEditor {

    [CustomEditor(typeof(SkillEditorGO))]
    public class SkillEditorInspector : Editor {

        SkillEditorGO editorGo;

        List<SerializedProperty> propertyList;

        void OnEnable() {
            if (target == null) return;

            editorGo = target as SkillEditorGO;

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

            propertyList.ForEach(p => {
                if (p.hasChildren) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(p, true);
                    EditorGUILayout.EndHorizontal();
                } else {
                    EditorGUILayout.PropertyField(p);
                }
            });

            serializedObject.ApplyModifiedProperties();
        }

    }


}