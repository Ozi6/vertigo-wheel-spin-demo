using UnityEditor;
using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Editor
{
    [CustomEditor(typeof(RewardItemSO))]
    public class RewardItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var rewardItem = (RewardItemSO)target;
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (rewardItem.Icon != null)
            {
                var rect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64), GUILayout.Height(64));
                GUI.DrawTexture(rect, rewardItem.Icon.texture, ScaleMode.ScaleToFit);
            }
            else
            {
                GUILayout.Box("No Icon", GUILayout.Width(64), GUILayout.Height(64));
            }
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_id"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_icon"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_value"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_tier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_weight"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
