using UnityEditor;
using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Editor
{
    [CustomPropertyDrawer(typeof(RewardPoolEntry))]
    public class RewardPoolEntryDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 6f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var itemProp = property.FindPropertyRelative("_rewardItem");
            var overrideProp = property.FindPropertyRelative("_overrideWeight");
            var weightProp = property.FindPropertyRelative("_weight");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            
            Rect line1 = new Rect(position.x, position.y + 2f, position.width, lineHeight);
            Rect line2 = new Rect(position.x, position.y + lineHeight + 4f, position.width, lineHeight);

            EditorGUI.PropertyField(line1, itemProp, new GUIContent(label.text + " (Reward Item)"));

            Rect overrideRect = new Rect(line2.x, line2.y, 140, lineHeight);
            EditorGUI.PropertyField(overrideRect, overrideProp, new GUIContent("Override Weight"));

            Rect weightRect = new Rect(line2.x + 145, line2.y, line2.width - 145, lineHeight);

            if (overrideProp.boolValue)
            {
                if (weightProp.floatValue <= 0.01f) weightProp.floatValue = 1f;
                EditorGUI.PropertyField(weightRect, weightProp, GUIContent.none);
            }
            else
            {
                GUI.enabled = false;
                float defaultWeight = 1f;
                if (itemProp.objectReferenceValue != null)
                {
                    var so = new SerializedObject(itemProp.objectReferenceValue);
                    defaultWeight = so.FindProperty("_weight").floatValue;
                }
                EditorGUI.FloatField(weightRect, GUIContent.none, defaultWeight);
                GUI.enabled = true;
            }

            EditorGUI.EndProperty();
        }
    }
}
