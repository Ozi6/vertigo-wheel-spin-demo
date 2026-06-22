using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Utility;

namespace WheelOfFortune.Views
{
    public sealed class DialogPresenter : MonoBehaviour, IDialogView
    {
        [SerializeField] private GameObject _bombScreen;
        [SerializeField] private Transform _bombRewardsGrid_value;
        [SerializeField] private Button _reviveButton_value;
        [SerializeField] private Button _giveUpButton_value;

        [SerializeField] private GameObject _collectScreen;
        [SerializeField] private Transform _collectRewardsGrid_value;
        [SerializeField] private Button _confirmButton_value;
        [SerializeField] private Button _cancelButton_value;

        [SerializeField] private RewardCard _rewardCardPrefab_value;

        private readonly List<RewardCard> _bombCards = new List<RewardCard>();
        private readonly List<RewardCard> _collectCards = new List<RewardCard>();
        private ComponentPool<RewardCard> _pool;

        private void Awake()
        {
            if (_rewardCardPrefab_value != null)
                _pool = new ComponentPool<RewardCard>(_rewardCardPrefab_value, "Pool_DialogRewardCards", 6);
        }

        public void ShowBombScreen(CollectedRewards lostRewards, Action onRevive, Action onGiveUp)
        {
            PopulateGrid(_bombRewardsGrid_value, _bombCards, lostRewards);
            SetListeners(_reviveButton_value, onRevive);
            SetListeners(_giveUpButton_value, onGiveUp);
            if (_bombScreen != null) _bombScreen.SetActive(true);
            if (_collectScreen != null) _collectScreen.SetActive(false);
        }

        public void ShowCollectConfirmScreen(CollectedRewards rewards, Action onConfirm, Action onCancel)
        {
            PopulateGrid(_collectRewardsGrid_value, _collectCards, rewards);
            SetListeners(_confirmButton_value, onConfirm);
            SetListeners(_cancelButton_value, onCancel);
            if (_collectScreen != null) _collectScreen.SetActive(true);
            if (_bombScreen != null) _bombScreen.SetActive(false);
        }

        public void Hide()
        {
            if (_bombScreen != null) _bombScreen.SetActive(false);
            if (_collectScreen != null) _collectScreen.SetActive(false);
            ClearGrid(_bombCards);
            ClearGrid(_collectCards);
        }

        private void PopulateGrid(Transform grid, List<RewardCard> cards, CollectedRewards rewards)
        {
            ClearGrid(cards);
            if (grid == null || _pool == null) return;

            var stacks = RewardStackBuilder.Build(rewards.Entries);
            foreach (var stack in stacks)
            {
                var card = _pool.Get(grid);
                card.name = $"ui_card_{stack.Item.Id}_value";
                card.Setup(stack);
                cards.Add(card);
            }
        }

        private void ClearGrid(List<RewardCard> cards)
        {
            if (_pool == null) return;
            foreach (var card in cards)
                if (card != null) _pool.Release(card);
            cards.Clear();
        }

        private void OnDestroy()
        {
            _pool?.Clear();
        }

        private void SetListeners(Button button, Action callback)
        {
            if (button == null) return;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => callback?.Invoke());
        }
    }
}