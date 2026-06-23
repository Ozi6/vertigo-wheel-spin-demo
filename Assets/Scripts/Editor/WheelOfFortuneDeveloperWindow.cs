using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using WheelOfFortune.Data;
using WheelOfFortune.Domain;
using WheelOfFortune.Factory;
using WheelOfFortune.Services;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Editor
{
    public sealed class WheelOfFortuneDeveloperWindow : EditorWindow
    {
        private int _activeTab = 0;
        private Vector2 _scrollPos1;
        private Vector2 _scrollPos2;

        private WheelConfigSO _selectedConfig;
        private int _simulationCount = 1000;
        private bool _hasRunSimulation = false;

        private int _simulatedSpins;
        private int _simulatedBombHits;
        private float _simulatedBombPercentage;
        private readonly List<SimulationRewardRow> _rewardRows = new List<SimulationRewardRow>();

        private struct SimulationRewardRow
        {
            public string RewardId;
            public int Count;
            public float Percentage;
            public float AverageMultiplier;
        }

        [MenuItem("Tools/Wheel of Fortune/Developer Window")]
        public static void ShowWindow()
        {
            GetWindow<WheelOfFortuneDeveloperWindow>("WoF Developer Tool");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Wheel of Fortune Developer Tool Suite", EditorStyles.boldLabel);
            _activeTab = GUILayout.Toolbar(_activeTab, new string[] { "Asset Hub", "Batch Spin Simulator" });

            EditorGUILayout.Space();

            if (_activeTab == 0)
                DrawAssetHub();
            else
                DrawSimulator();
        }

        private void DrawAssetHub()
        {
            _scrollPos1 = EditorGUILayout.BeginScrollView(_scrollPos1);

            EditorGUILayout.LabelField("Global Configurations", EditorStyles.boldLabel);
            var settingsGuids = AssetDatabase.FindAssets("t:GameSettingsSO");
            if(settingsGuids.Length > 0)
            {
                foreach (var guid in settingsGuids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var settings = AssetDatabase.LoadAssetAtPath<GameSettingsSO>(path);
                    if (settings != null)
                    {
                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        EditorGUILayout.LabelField($"Settings: {settings.name}", GUILayout.Width(250));
                        if (GUILayout.Button("Select Asset", GUILayout.Width(110)))
                            Selection.activeObject = settings;
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
                EditorGUILayout.HelpBox("No GameSettingsSO found in the project.", MessageType.Warning);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Zone Configurations", EditorStyles.boldLabel);

            var zoneGuids = AssetDatabase.FindAssets("t:ZoneConfigSO");
            if (zoneGuids.Length > 0)
            {
                foreach (var guid in zoneGuids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var zone = AssetDatabase.LoadAssetAtPath<ZoneConfigSO>(path);
                    if (zone != null)
                    {
                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        string wheelName = zone.WheelConfig != null ? zone.WheelConfig.name : "None";
                        EditorGUILayout.LabelField($"Zone: {zone.name} ({zone.ZoneType}) - Wheel: {wheelName}");
                        if (GUILayout.Button("Select", GUILayout.Width(90)))
                            Selection.activeObject = zone;
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No ZoneConfigSO found in the project.", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSimulator()
        {
            _scrollPos2 = EditorGUILayout.BeginScrollView(_scrollPos2);

            EditorGUILayout.LabelField("Monte Carlo Spin Simulator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Run design-time simulations to test probabilities of a wheel config under live math conditions.", MessageType.Info);

            EditorGUILayout.Space();
            _selectedConfig = (WheelConfigSO)EditorGUILayout.ObjectField("Wheel Config To Test", _selectedConfig, typeof(WheelConfigSO), false);
            _simulationCount = EditorGUILayout.IntField("Simulation Run Count", _simulationCount);
            if (_simulationCount < 10) _simulationCount = 10;
            if (_simulationCount > 100000) _simulationCount = 100000;

            EditorGUILayout.Space();

            if (GUILayout.Button("Run Batch Spin Simulations", GUILayout.Height(30)))
            {
                if (_selectedConfig == null)
                    EditorUtility.DisplayDialog("Validation Error", "Please drag in a valid WheelConfigSO asset to simulate.", "OK");
                else
                    RunSimulation();
            }

            if (_hasRunSimulation)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Simulation Output Summary", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Total Runs: {_simulatedSpins}");
                EditorGUILayout.LabelField($"Bomb Landings: {_simulatedBombHits} ({_simulatedBombPercentage:F2}% of spins)");

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Reward Items Probability Metrics:", EditorStyles.boldLabel);

                if (_rewardRows.Count > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Reward Identifier", EditorStyles.miniBoldLabel, GUILayout.Width(150));
                    EditorGUILayout.LabelField("Hits Count", EditorStyles.miniBoldLabel, GUILayout.Width(90));
                    EditorGUILayout.LabelField("Simulated Rate", EditorStyles.miniBoldLabel, GUILayout.Width(110));
                    EditorGUILayout.LabelField("Avg Multiplier", EditorStyles.miniBoldLabel, GUILayout.Width(110));
                    EditorGUILayout.EndHorizontal();

                    foreach (var row in _rewardRows)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(row.RewardId, GUILayout.Width(150));
                        EditorGUILayout.LabelField(row.Count.ToString(), GUILayout.Width(90));
                        EditorGUILayout.LabelField($"{row.Percentage:F2}%", GUILayout.Width(110));
                        EditorGUILayout.LabelField($"x{row.AverageMultiplier:F2}", GUILayout.Width(110));
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No reward hits recorded in this batch simulation run.");
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }

        private void RunSimulation()
        {
            if (_selectedConfig == null) return;

            _simulatedSpins = _simulationCount;
            _simulatedBombHits = 0;
            _rewardRows.Clear();

            var drawer = new SliceDrawer();
            var bombInjector = new BombInjector();
            var rewardCounts = new Dictionary<string, int>();
            var rewardMultipliersTotal = new Dictionary<string, int>();

            for (int i = 0; i < _simulatedSpins; i++)
            {
                var slices = drawer.DrawSlices(_selectedConfig);
                int bombIndex = -1;
                if (_selectedConfig.HasBomb)
                    bombIndex = bombInjector.InjectBomb(slices);
                var runtimeSlices = new RuntimeSlice[slices.Length];
                for (int j = 0; j < slices.Length; j++)
                {
                    var def = slices[j];
                    var rData = def.RewardItem != null ? def.RewardItem.ToData() : default;
                    runtimeSlices[j] = new RuntimeSlice(rData, def.Multiplier, def.IsBomb, def.Weight);
                }
                var wheelData = new RuntimeWheelData(runtimeSlices, bombIndex, _selectedConfig.HasBomb, _selectedConfig.IsWeighted);
                IWheelSpinStrategy strategy = wheelData.IsWeighted 
                    ? new WeightedSpinStrategy() 
                    : new RandomSpinStrategy();
                int winIndex = strategy.GetWinningIndex(wheelData);
                var winningSlice = wheelData.Slices[winIndex];
                if (winningSlice.IsBomb)
                    _simulatedBombHits++;
                else
                {
                    string id = !string.IsNullOrEmpty(winningSlice.Reward.Id) ? winningSlice.Reward.Id : "Unknown";
                    if (!rewardCounts.ContainsKey(id))
                    {
                        rewardCounts[id] = 0;
                        rewardMultipliersTotal[id] = 0;
                    }
                    rewardCounts[id]++;
                    rewardMultipliersTotal[id] += winningSlice.Multiplier;
                }
            }
            _simulatedBombPercentage = ((float)_simulatedBombHits / _simulatedSpins) * 100f;
            foreach (var kp in rewardCounts)
            {
                float pct = ((float)kp.Value / _simulatedSpins) * 100f;
                float avgMult = (float)rewardMultipliersTotal[kp.Key] / kp.Value;
                _rewardRows.Add(new SimulationRewardRow
                {
                    RewardId = kp.Key,
                    Count = kp.Value,
                    Percentage = pct,
                    AverageMultiplier = avgMult
                });
            }
            _rewardRows.Sort((a, b) => string.Compare(a.RewardId, b.RewardId, System.StringComparison.Ordinal));
            _hasRunSimulation = true;
        }
    } 
}
