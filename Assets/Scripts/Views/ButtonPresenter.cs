using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Controller;

namespace WheelOfFortune.Views
{
    public sealed class ButtonPresenter : MonoBehaviour
    {
        [SerializeField] private Button _spinButton_value;
        [SerializeField] private Button _collectButton_value;

        private GameController _gameController;

        public void Init(GameController gameController)
        {
            _gameController = gameController;

            if (_spinButton_value != null)
                _spinButton_value.onClick.AddListener(() => _gameController.ExecuteSpin());

            if (_collectButton_value != null)
                _collectButton_value.onClick.AddListener(() => _gameController.ExecuteCollect());
        }
    }
}