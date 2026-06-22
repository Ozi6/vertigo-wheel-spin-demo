using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Controller;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Views
{
    public sealed class ButtonPresenter : MonoBehaviour, IButtonView
    {
        [SerializeField] private Button _spinButton_value;
        [SerializeField] private Button _collectButton_value;

        private GameController _gameController;

        public void Init(GameController gameController)
        {
            _gameController = gameController;
            SetCollectVisible(false);
        }

        private void OnEnable()
        {
            if (_spinButton_value != null)
                _spinButton_value.onClick.AddListener(HandleSpinClicked);

            if (_collectButton_value != null)
                _collectButton_value.onClick.AddListener(HandleCollectClicked);
        }

        private void OnDisable()
        {
            if (_spinButton_value != null)
                _spinButton_value.onClick.RemoveListener(HandleSpinClicked);

            if (_collectButton_value != null)
                _collectButton_value.onClick.RemoveListener(HandleCollectClicked);
        }

        private void HandleSpinClicked() => _gameController?.ExecuteSpin();
        private void HandleCollectClicked() => _gameController?.ExecuteCollect();

        public void SetSpinInteractable(bool interactable)
        {
            if (_spinButton_value != null)
                _spinButton_value.interactable = interactable;
        }

        public void SetCollectVisible(bool visible)
        {
            if (_collectButton_value != null)
                _collectButton_value.gameObject.SetActive(visible);
        }

        private void OnValidate()
        {
            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                string nameLower = btn.name.ToLower();
                if (nameLower.Contains("spin"))
                    _spinButton_value = btn;
                else if (nameLower.Contains("collect"))
                    _collectButton_value = btn;
            }
        }
    }
}