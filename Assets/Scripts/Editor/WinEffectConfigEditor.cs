using UnityEditor;
using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Editor
{ 
    [CustomEditor(typeof(WinEffectConfig))]
    public sealed class WinEffectConfigEditor : UnityEditor.Editor
    { 
        private static bool _zoomFolded = true;
        private static bool _settleFolded = true;
        private static bool _fadeFolded = true;
        private static bool _bgFolded = true;
        private static bool _burstFolded = true;

        public override void OnInspectorGUI()
        { 
            serializedObject.Update();

            EditorGUILayout.LabelField("Win Presentation Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _zoomFolded = EditorGUILayout.BeginFoldoutHeaderGroup(_zoomFolded, "Zoom Anim Properties");
            if (_zoomFolded)
            { 
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_zoomDuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_zoomScalePeak"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_zoomEase"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_zoomWorldOffset"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            _settleFolded = EditorGUILayout.BeginFoldoutHeaderGroup(_settleFolded, "Settle Visual Properties");
            if (_settleFolded)
            { 
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_settleScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_settleDuration"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            _fadeFolded = EditorGUILayout.BeginFoldoutHeaderGroup(_fadeFolded, "Slice Fade Configuration");
            if (_fadeFolded)
            { 
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_fadeDuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_fadeEase"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_reelBackTriggerFraction"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            _bgFolded = EditorGUILayout.BeginFoldoutHeaderGroup(_bgFolded, "Background Spinner Configuration");
            if (_bgFolded)
            { 
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_backgroundSprite"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_backgroundFallbackColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_backgroundSize"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_backgroundSpinDuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_backgroundFadeInDuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_backgroundFadeOutDuration"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            _burstFolded = EditorGUILayout.BeginFoldoutHeaderGroup(_burstFolded, "Multiplier Icon Fly Burst Configuration");
            if (_burstFolded)
            { 
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_iconSize"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_burstRadius"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_popDuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_burstMoveDuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_flyDuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_flyStagger"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_flyArcStrength"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_flyEase"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        } 
    } 
}
