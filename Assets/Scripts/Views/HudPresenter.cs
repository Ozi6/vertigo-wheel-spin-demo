using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Utility;

namespace WheelOfFortune.Views
{
    public sealed class HudPresenter : MonoBehaviour, IHudView
    {
        [SerializeField] private RectTransform _zoneStrip_value;
        [SerializeField] private TextMeshProUGUI _zoneCellPrefab_value;
        [SerializeField] private Transform _rewardsContainer_value;
        [SerializeField] private RewardCard _rewardCardPrefab_value;
        [SerializeField] private TextMeshProUGUI _currencyDisplay_value;

        [SerializeField, Min(1)] private int _safeZoneInterval = 5;
        [SerializeField, Min(1)] private int _superZoneInterval = 30;
        [SerializeField, Min(1)] private int _totalZones = 30;
        [SerializeField, Min(1f)] private float _cellWidth = 64f;
        [SerializeField, Min(0.01f)] private float _scrollDuration = 0.4f;
        [SerializeField] private Ease _scrollEase = Ease.OutCubic;

        [SerializeField] private Color _colorNormal = new Color(0.72f, 0.45f, 0.20f);
        [SerializeField] private Color _colorSafe = new Color(0.75f, 0.75f, 0.75f);
        [SerializeField] private Color _colorSuper = new Color(1.00f, 0.84f, 0.20f);
        [SerializeField] private Color _colorCurrent = new Color(0.20f, 0.85f, 0.25f);

        private readonly List<TextMeshProUGUI> _cells = new List<TextMeshProUGUI>();
        private readonly List<RewardCard> _rewardCards = new List<RewardCard>();
        private readonly Dictionary<string, RewardCard> _cardById = new Dictionary<string, RewardCard>();
        private Tweener _scrollTween;

        private void Start()
        {
            _zoneStrip_value.anchorMin = new Vector2(0.5f, 0.5f);
            _zoneStrip_value.anchorMax = new Vector2(0.5f, 0.5f);
            _zoneStrip_value.pivot = new Vector2(0.5f, 0.5f);

            BuildStrip();
            SnapStripTo(1);
            RefreshColors(1);
        }

        public Transform GetRewardsPanelTarget() => _rewardsContainer_value;

        public void UpdateZoneDisplay(ZoneProgressModel progress)
        {
            RefreshColors(progress.ZoneNumber);
            AnimateStripTo(progress.ZoneNumber);
        }

        public void UpdateRewardsDisplay(CollectedRewards rewards)
        {
            foreach (var card in _rewardCards)
                if (card != null) Destroy(card.gameObject);
            _rewardCards.Clear();
            _cardById.Clear();

            if (_rewardCardPrefab_value == null || _rewardsContainer_value == null) return;

            var stacks = RewardStackBuilder.Build(rewards.Entries);
            foreach (var stack in stacks)
            {
                var card = Instantiate(_rewardCardPrefab_value, _rewardsContainer_value);
                card.name = $"ui_card_reward_{stack.Item.Id}_value";
                card.Setup(stack);
                _rewardCards.Add(card);
                _cardById[stack.Item.Id] = card;
            }
        }

        public void UpdateCurrencyDisplay(int balance)
        {
            if (_currencyDisplay_value != null)
                _currencyDisplay_value.text = balance.ToString();
        }

        public void InitializeNewRewardCard(CollectedRewards rewards, string newItemId)
        {
            if (_rewardCardPrefab_value == null || _rewardsContainer_value == null) return;

            var stacks = RewardStackBuilder.Build(rewards.Entries);
            foreach (var stack in stacks)
            {
                if (stack.Item == null || stack.Item.Id != newItemId) continue;
                if (_cardById.ContainsKey(newItemId)) continue;

                var card = Instantiate(_rewardCardPrefab_value, _rewardsContainer_value);
                card.name = $"ui_card_reward_{stack.Item.Id}_value";
                card.InitializeEmpty(stack);
                _rewardCards.Add(card);
                _cardById[newItemId] = card;
                break;
            }
        }

        public Action<int> BuildIconArrivedCallback(string itemId, int previousMultiplier, int rewardMultiplier)
        {
            return arrived =>
            {
                if (!_cardById.TryGetValue(itemId, out var card) || card == null)
                    return;

                card.SetMultiplier(previousMultiplier + arrived);
            };
        }

        public Action BuildFinalMultiplierCallback(string itemId, int finalValue)
        {
            return () =>
            {
                if (!_cardById.TryGetValue(itemId, out var card) || card == null)
                    return;

                card.SetMultiplier(finalValue);
            };
        }

        private void BuildStrip()
        {
            foreach (Transform child in _zoneStrip_value)
                Destroy(child.gameObject);
            _cells.Clear();

            for (int i = 1; i <= _totalZones; i++)
            {
                var cell = Instantiate(_zoneCellPrefab_value, _zoneStrip_value);
                cell.name = $"ui_text_zone_{i:D3}_value";
                cell.text = i.ToString();
                cell.enableWordWrapping = false;
                cell.overflowMode = TextOverflowModes.Overflow;

                var rt = cell.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2((i - 1) * _cellWidth - 8f, 0f);
                rt.sizeDelta = new Vector2(_cellWidth, _cellWidth);

                _cells.Add(cell);
            }
        }

        private void SnapStripTo(int zone)
        {
            _zoneStrip_value.anchoredPosition = new Vector2(TargetX(zone), 0f);
        }

        private void AnimateStripTo(int zone)
        {
            _scrollTween?.Kill();
            float endX = TargetX(zone);
            _scrollTween = DOTween.To(
                () => _zoneStrip_value.anchoredPosition.x,
                x => _zoneStrip_value.anchoredPosition = new Vector2(x, _zoneStrip_value.anchoredPosition.y),
                endX,
                _scrollDuration
            ).SetEase(_scrollEase);
        }

        private float TargetX(int zone) => -(zone - 1) * _cellWidth;

        private void RefreshColors(int currentZone)
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                int zoneNumber = i + 1;
                _cells[i].color = zoneNumber == currentZone ? _colorCurrent : GetZoneColor(zoneNumber);
            }
        }

        private Color GetZoneColor(int zoneNumber)
        {
            if (_superZoneInterval > 0 && zoneNumber % _superZoneInterval == 0) return _colorSuper;
            if (_safeZoneInterval > 0 && zoneNumber % _safeZoneInterval == 0) return _colorSafe;
            return _colorNormal;
        }

        private void OnValidate()
        {
            if (_safeZoneInterval < 1) _safeZoneInterval = 1;
            if (_superZoneInterval < 1) _superZoneInterval = 1;
            if (_totalZones < 1) _totalZones = 1;
            if (_cellWidth < 1f) _cellWidth = 1f;
            if (_scrollDuration < 0.01f) _scrollDuration = 0.01f;
        }
    }
}