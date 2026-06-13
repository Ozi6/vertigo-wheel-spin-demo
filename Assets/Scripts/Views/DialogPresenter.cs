using System;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Views
{
    public sealed class DialogPresenter : MonoBehaviour, IDialogView
    {
        [SerializeField] private GameObject _bombScreen;
        [SerializeField] private Button _reviveButton_value;
        [SerializeField] private Button _giveUpButton_value;

        [SerializeField] private GameObject _collectScreen;
        [SerializeField] private Button _confirmButton_value;
        [SerializeField] private Button _cancelButton_value;

        public void ShowBombScreen(Action onRevive, Action onGiveUp)
        {
            SetBombListeners(onRevive, onGiveUp);

            if (_bombScreen != null) _bombScreen.SetActive(true);
            if (_collectScreen != null) _collectScreen.SetActive(false);
        }

        public void ShowCollectConfirmScreen(Action onConfirm, Action onCancel)
        {
            SetCollectListeners(onConfirm, onCancel);

            if (_collectScreen != null) _collectScreen.SetActive(true);
            if (_bombScreen != null) _bombScreen.SetActive(false);
        }

        public void Hide()
        {
            if (_bombScreen != null) _bombScreen.SetActive(false);
            if (_collectScreen != null) _collectScreen.SetActive(false);
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