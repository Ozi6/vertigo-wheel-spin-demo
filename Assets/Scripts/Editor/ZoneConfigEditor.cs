using UnityEditor;
using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.Editor
{ 
    [CustomEditor(typeof(ZoneConfigSO))]
    public sealed class ZoneConfigEditor : UnityEditor.Editor
    { 
        private SerializedProperty _zoneTypeProp;
        private SerializedProperty _wheelConfigProp;
        private SerializedProperty _wheelSpriteProp;
        private SerializedProperty _arrowSpriteProp;

        private void OnEnable()
        { 
            _zoneTypeProp = serializedObject.FindProperty("_zoneType");
            _wheelConfigProp = serializedObject.FindProperty("_wheelConfig");
            _wheelSpriteProp = serializedObject.FindProperty("_wheelSprite");
            _arrowSpriteProp = serializedObject.FindProperty("_arrowSprite");
        }

        public override void OnInspectorGUI()
        { 
            serializedObject.Update();

            EditorGUILayout.LabelField("Zone Configuration Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_zoneTypeProp);
            EditorGUILayout.PropertyField(_wheelConfigProp);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Visual Reference Assets", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_wheelSpriteProp);
            EditorGUILayout.PropertyField(_arrowSpriteProp);

            if (_wheelSpriteProp.objectReferenceValue != null || _arrowSpriteProp.objectReferenceValue != null)
            { 
                EditorGUILayout.Space();
                Rect previewRect = EditorGUILayout.GetControlRect(false, 80);
                float halfWidth = previewRect.width / 2f;

                if (_wheelSpriteProp.objectReferenceValue != null)
                { 
                    Sprite wheelSprite = (Sprite)_wheelSpriteProp.objectReferenceValue;
                    Rect leftRect = new Rect(previewRect.x, previewRect.y, halfWidth - 5, previewRect.height);
                    GUI.Box(leftRect, "Wheel Sprite Preview");
                    if (wheelSprite.texture != null)
                    { 
                        GUI.DrawTexture(leftRect, wheelSprite.texture, ScaleMode.ScaleToFit);
                    }
                }

                if (_arrowSpriteProp.objectReferenceValue != null)
                { 
                    Sprite arrowSprite = (Sprite)_arrowSpriteProp.objectReferenceValue;
                    Rect rightRect = new Rect(previewRect.x + halfWidth + 5, previewRect.y, halfWidth - 5, previewRect.height);
                    GUI.Box(rightRect, "Arrow Sprite Preview");
                    if (arrowSprite.texture != null)
                    { 
                        GUI.DrawTexture(rightRect, arrowSprite.texture, ScaleMode.ScaleToFit);
                    }
                }
            }

            if (_wheelConfigProp.objectReferenceValue != null)
            { 
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Linked Wheel Slices & Settings", EditorStyles.boldLabel);

                var wheelConfig = (WheelConfigSO)_wheelConfigProp.objectReferenceValue;
                EditorGUILayout.LabelField($"Slices Target Count: {wheelConfig.SliceCount}");
                EditorGUILayout.LabelField($"Contains Bomb: {wheelConfig.HasBomb}");
                EditorGUILayout.LabelField($"Uses Weighted Calculations: {wheelConfig.IsWeighted}");
                EditorGUILayout.LabelField($"Multipliers: {wheelConfig.MinMultiplier} to {wheelConfig.MaxMultiplier}");

                if (wheelConfig.RewardPool != null && wheelConfig.RewardPool.Length > 0)
                { 
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Reward Pool Elements:", EditorStyles.miniBoldLabel);
                    foreach (var entry in wheelConfig.RewardPool)
                    { 
                        string nameLabel = entry.RewardItem != null ? entry.RewardItem.Id : "Bomb/Empty";
                        float weight = entry.Weight;
                        EditorGUILayout.LabelField($"- {nameLabel} (Calculated Weight: {weight})", EditorStyles.miniLabel);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }
    } 
}
