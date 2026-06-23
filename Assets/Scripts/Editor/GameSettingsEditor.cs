using UnityEditor;
using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Editor
{
    [CustomEditor(typeof(GameSettingsSO))]
    public class GameSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Wheel Of Fortune - Global Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Reset to Defaults", GUILayout.Height(30)))
            {
                Undo.RecordObject(target, "Reset Game Settings");
                var settings = (GameSettingsSO)target;
                var so = new SerializedObject(settings);
                so.FindProperty("_safeZoneInterval").intValue = 5;
                so.FindProperty("_superZoneInterval").intValue = 30;
                so.FindProperty("_startingReviveCost").intValue = 50;
                so.FindProperty("_startingCurrencyBalance").intValue = 1000;
                so.ApplyModifiedProperties();
            }
        }
    }
}
