using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Views
{
    public sealed class DialogPresenter : MonoBehaviour, IDialogView
    {
        [SerializeField] private GameObject _bombScreen;
        [SerializeField] private Button _reviveButton_value;
        [SerializeField] private Button _giveUpButton_value;

        [SerializeField] private GameObject _collectScreen;
        [SerializeField] private Transform _collectRewardsGrid_value;
        [SerializeField] private RewardCard _rewardCardPrefab_value;
        [SerializeField] private Button _confirmButton_value;
        [SerializeField] private TextMeshProUGUI _confirmLabel_value;
        [SerializeField] private Button _cancelButton_value;

        private readonly List<RewardCard> _collectCards = new List<RewardCard>();

        public void ShowBombScreen(Action onRevive, Action onGiveUp)
        {
            SetBombListeners(onRevive, onGiveUp);
            if (_bombScreen != null) _bombScreen.SetActive(true);
            if (_collectScreen != null) _collectScreen.SetActive(false);
        }

        public void ShowCollectConfirmScreen(CollectedRewards rewards, Action onConfirm, Action onCancel)
        {
            PopulateCollectGrid(rewards);
            SetCollectListeners(onConfirm, onCancel);
            if (_confirmLabel_value != null) _confirmLabel_value.text = "COLLECT & LEAVE";
            if (_collectScreen != null) _collectScreen.SetActive(true);
            if (_bombScreen != null) _bombScreen.SetActive(false);
        }

        public void Hide()
        {
            if (_bombScreen != null) _bombScreen.SetActive(false);
            if (_collectScreen != null) _collectScreen.SetActive(false);
            ClearCollectGrid();
        }

        private void PopulateCollectGrid(CollectedRewards rewards)
        {
            ClearCollectGrid();
            if (_collectRewardsGrid_value == null || _rewardCardPrefab_value == null) return;

            foreach (var item in rewards.Items)
            {
                var card = Instantiate(_rewardCardPrefab_value, _collectRewardsGrid_value);
                card.name = "ui_card_collect_reward_value";
                var icon = item != null ? item.Icon : null;
                var label = item != null ? item.Value.ToString("F0") : string.Empty;
                card.Setup(icon, label);
                _collectCards.Add(card);
            }
        }

        private void ClearCollectGrid()
        {
            foreach (var card in _collectCards)
                if (card != null) Destroy(card.gameObject);
            _collectCards.Clear();
        }

        private void SetBombListeners(Action onRevive, Action onGiveUp)
        {
            if (_reviveButton_value != null)
            {
                _reviveButton_value.onClick.RemoveAllListeners();
                _reviveButton_value.onClick.AddListener(() => onRevive?.Invoke());
            }

            if (_giveUpButton_value != null)
            {
                _giveUpButton_value.onClick.RemoveAllListeners();
                _giveUpButton_value.onClick.AddListener(() => onGiveUp?.Invoke());
            }
        }

        private void SetCollectListeners(Action onConfirm, Action onCancel)
        {
            if (_confirmButton_value != null)
            {
                _confirmButton_value.onClick.RemoveAllListeners();
                _confirmButton_value.onClick.AddListener(() => onConfirm?.Invoke());
            }

            if (_cancelButton_value != null)
            {
                _cancelButton_value.onClick.RemoveAllListeners();
                _cancelButton_value.onClick.AddListener(() => onCancel?.Invoke());
            }
        }
    }
}