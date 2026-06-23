using UnityEditor;
using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Editor
{
    [CustomEditor(typeof(WheelConfigSO))]
    public class WheelConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Wheel Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            SerializedProperty poolProp = serializedObject.FindProperty("_rewardPool");
            EditorGUILayout.PropertyField(poolProp, true);

            var wheelConfig = (WheelConfigSO)target;
            if (wheelConfig.RewardPool != null && wheelConfig.RewardPool.Length > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Expected Drop Probabilities", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                float totalPoolWeight = 0f;
                if (wheelConfig.IsWeighted)
                {
                    foreach (var entry in wheelConfig.RewardPool)
                        totalPoolWeight += entry.Weight;
                }

                for (int i = 0; i < wheelConfig.RewardPool.Length; i++)
                {
                    float chance = 0f;
                    var entry = wheelConfig.RewardPool[i];

                    if (wheelConfig.IsWeighted)
                    {
                        if (totalPoolWeight > 0f)
                            chance = (entry.Weight / totalPoolWeight) * 100f;
                    }
                    else
                    {
                        chance = (1f / wheelConfig.RewardPool.Length) * 100f;
                    }

                    string name = entry.RewardItem == null ? "Bomb / Empty" : $"{entry.RewardItem.name} ({entry.RewardItem.Id})";
                    EditorGUILayout.LabelField(name, $"{chance:F1}%");
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_sliceCount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_hasBomb"));
            if (wheelConfig.HasBomb)
            {
                EditorGUILayout.LabelField("* Bomb injection overwrites 1 random slice at runtime.", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_isWeighted"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Multiplier Range", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_minMultiplier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxMultiplier"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
