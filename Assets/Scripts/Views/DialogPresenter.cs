using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Utility;

namespace WheelOfFortune.Views
{
    public sealed class DialogPresenter : MonoBehaviour, IDialogView
    {
        #region Inspector Fields
        [SerializeField] private GameObject _bombScreen;
        [SerializeField] private Transform _bombRewardsGrid_value;
        [SerializeField] private Button _reviveButton_value;
        [SerializeField] private TextMeshProUGUI _reviveCostDisplay_value;
        [SerializeField] private Button _giveUpButton_value;

        [SerializeField] private GameObject _collectScreen;
        [SerializeField] private Transform _collectRewardsGrid_value;
        [SerializeField] private Button _confirmButton_value;
        [SerializeField] private Button _cancelButton_value;

        [SerializeField] private RewardCard _rewardCardPrefab_value;
        #endregion

        #region Private Fields
        private readonly List<RewardCard> _bombCards = new List<RewardCard>();
        private readonly List<RewardCard> _collectCards = new List<RewardCard>();
        private ComponentPool<RewardCard> _pool;
        private IRewardRegistry _registry;
        private IEventBus _eventBus;
        private int _currentCost;
        private int _currentBalance;
        private bool _isInitialized;
        private bool _isSubscribed;

        private Action _onReviveCallback;
        private Action _onGiveUpCallback;
        private Action _onConfirmCallback;
        private Action _onCancelCallback;
        #endregion

        #region Initialization & Lifecycle
        public void Initialize(IEventBus eventBus, IRewardRegistry registry)
        {
            _eventBus = eventBus;
            _registry = registry;
            _isInitialized = true;
            
            if (isActiveAndEnabled)
                SubscribeEvents();
        }

        private void Awake()
        {
            if (_rewardCardPrefab_value != null)
                _pool = new ComponentPool<RewardCard>(_rewardCardPrefab_value, "Pool_DialogRewardCards", 6);
        }

        private void OnEnable()
        {
            if (_isInitialized) SubscribeEvents();

            if (_reviveButton_value != null)
                _reviveButton_value.onClick.AddListener(HandleReviveClicked);
            if (_giveUpButton_value != null)
                _giveUpButton_value.onClick.AddListener(HandleGiveUpClicked);
            if (_confirmButton_value != null)
                _confirmButton_value.onClick.AddListener(HandleConfirmClicked);
            if (_cancelButton_value != null)
                _cancelButton_value.onClick.AddListener(HandleCancelClicked);
        }

        private void OnDisable()
        {
            UnsubscribeEvents();

            if (_reviveButton_value != null)
                _reviveButton_value.onClick.RemoveListener(HandleReviveClicked);
            if (_giveUpButton_value != null)
                _giveUpButton_value.onClick.RemoveListener(HandleGiveUpClicked);
            if (_confirmButton_value != null)
                _confirmButton_value.onClick.RemoveListener(HandleConfirmClicked);
            if (_cancelButton_value != null)
                _cancelButton_value.onClick.RemoveListener(HandleCancelClicked);
        }

        private void OnDestroy()
        {
            _pool?.Clear();
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (_isSubscribed || _eventBus == null) return;
            _eventBus.Subscribe<Events.OnBalanceChange>(OnBalanceChanged);
            _eventBus.Subscribe<Events.OnReviveCostChanged>(OnReviveCostChanged);
            _isSubscribed = true;
        }

        private void UnsubscribeEvents()
        {
            if (!_isSubscribed || _eventBus == null) return;
            _eventBus.Unsubscribe<Events.OnBalanceChange>(OnBalanceChanged);
            _eventBus.Unsubscribe<Events.OnReviveCostChanged>(OnReviveCostChanged);
            _isSubscribed = false;
        }
        #endregion

        #region UI Event Handlers
        private void HandleReviveClicked() => _onReviveCallback?.Invoke();
        private void HandleGiveUpClicked() => _onGiveUpCallback?.Invoke();
        private void HandleConfirmClicked() => _onConfirmCallback?.Invoke();
        private void HandleCancelClicked() => _onCancelCallback?.Invoke();
        #endregion

        #region Public Interface
        public void ShowBombScreen(CollectedRewards lostRewards, Action onRevive, Action onGiveUp)
        {
            PopulateGrid(_bombRewardsGrid_value, _bombCards, lostRewards);
            _onReviveCallback = onRevive;
            _onGiveUpCallback = onGiveUp;

            if (_bombScreen != null) _bombScreen.SetActive(true);
            if (_collectScreen != null) _collectScreen.SetActive(false);
        }

        public void ShowCollectConfirmScreen(CollectedRewards rewards, Action onConfirm, Action onCancel)
        {
            PopulateGrid(_collectRewardsGrid_value, _collectCards, rewards);
            _onConfirmCallback = onConfirm;
            _onCancelCallback = onCancel;

            if (_bombScreen != null) _bombScreen.SetActive(false);
            if (_collectScreen != null) _collectScreen.SetActive(true);
        }

        public void Hide()
        {
            if (_bombScreen != null) _bombScreen.SetActive(false);
            if (_collectScreen != null) _collectScreen.SetActive(false);

            _onReviveCallback = null;
            _onGiveUpCallback = null;
            _onConfirmCallback = null;
            _onCancelCallback = null;

            ClearGrid(_bombCards);
            ClearGrid(_collectCards);
        }
        #endregion

        #region Private Methods
        private void OnBalanceChanged(Events.OnBalanceChange evt)
        {
            _currentBalance = evt.NewBalance;
            UpdateInteractable();
        }

        private void OnReviveCostChanged(Events.OnReviveCostChanged evt)
        {
            _currentCost = evt.NextCost;
            if (_reviveCostDisplay_value != null)
                _reviveCostDisplay_value.text = _currentCost.ToString();
            UpdateInteractable();
        }

        private void UpdateInteractable()
        {
            if (_reviveButton_value != null)
                _reviveButton_value.interactable = _currentBalance >= _currentCost;
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
                var icon = _registry?.GetReward(stack.Item.Id)?.Icon;
                card.Setup(stack, icon);
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

        private void OnValidate()
        {
            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                string nameLower = btn.name.ToLower();
                if (nameLower.Contains("revive"))
                    _reviveButton_value = btn;
                else if (nameLower.Contains("give") || nameLower.Contains("up"))
                    _giveUpButton_value = btn;
                else if (nameLower.Contains("confirm") || nameLower.Contains("accept"))
                    _confirmButton_value = btn;
                else if (nameLower.Contains("cancel") || nameLower.Contains("close"))
                    _cancelButton_value = btn;
            }

            var transforms = GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                string nameLower = t.name.ToLower();
                if (nameLower.Contains("bomb") && (nameLower.Contains("grid") || nameLower.Contains("container")))
                    _bombRewardsGrid_value = t;
                else if (nameLower.Contains("collect") && (nameLower.Contains("grid") || nameLower.Contains("container")))
                    _collectRewardsGrid_value = t;
            }

            var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var txt in texts)
            {
                string nameLower = txt.name.ToLower();
                if (nameLower.Contains("cost") || nameLower.Contains("revive"))
                    _reviveCostDisplay_value = txt;
            }
        }
        #endregion
    }
}
