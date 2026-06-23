using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Views
{
    public sealed class ButtonPresenter : MonoBehaviour, IButtonView
    {
        [SerializeField] private Button _spinButton_value;
        [SerializeField] private Button _collectButton_value;

        private ICommand _spinCommand;
        private ICommand _collectCommand;

        private void Start()
        {
            SetCollectVisible(false);
        }

        public void SetCommands(ICommand spinCommand, ICommand collectCommand)
        {
            _spinCommand = spinCommand;
            _collectCommand = collectCommand;
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

        private void HandleSpinClicked() => _spinCommand?.Execute();
        private void HandleCollectClicked() => _collectCommand?.Execute();

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