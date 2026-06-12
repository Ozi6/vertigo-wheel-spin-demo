using System;
using UnityEngine;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Views
{
    public sealed class DialogPresenter : MonoBehaviour, IDialogView
    {
        [SerializeField] private GameObject _bombScreen;
        [SerializeField] private GameObject _collectScreen;

        public void ShowBombScreen(Action onRevive, Action onGiveUp)
        {
            if (_bombScreen != null) _bombScreen.SetActive(true);
            if (_collectScreen != null) _collectScreen.SetActive(false);
        }

        public void ShowCollectConfirmScreen(Action onConfirm, Action onCancel)
        {
            if (_collectScreen != null) _collectScreen.SetActive(true);
            if (_bombScreen != null) _bombScreen.SetActive(false);
        }

        public void Hide()
        {
            if (_bombScreen != null) _bombScreen.SetActive(false);
            if (_collectScreen != null) _collectScreen.SetActive(false);
        }
    }
}
