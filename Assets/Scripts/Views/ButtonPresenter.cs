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

        public void Init(GameController gameController)
        {
            if (_spinButton_value != null)
                _spinButton_value.onClick.AddListener(() => gameController.ExecuteSpin());

            if (_collectButton_value != null)
                _collectButton_value.onClick.AddListener(() => gameController.ExecuteCollect());

            SetCollectVisible(false);
        }

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
    }
}