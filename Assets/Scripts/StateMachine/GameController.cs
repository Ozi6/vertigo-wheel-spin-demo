using UnityEngine;
using WheelOfFortune.Commands;
using WheelOfFortune.Data;
using WheelOfFortune.Factory;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Services;
using WheelOfFortune.StateMachine;

namespace WheelOfFortune.Controller
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private WinEffectConfig _winEffectConfig_value;

        private GameContext _ctx;
        private IGameState _currentState;
        private IdleState _idleState;
        private SpinCommand _spinCommand;
        private CollectCommand _collectCommand;
        private ICurrencyService _currencyService;

        public void Init(
            IZoneService zoneService,
            ISpinService spinService,
            IRewardService rewardService,
            IWheelView wheelView,
            IHudView hudView,
            IDialogView dialogView,
            IButtonView buttonView,
            IWheelFactory wheelFactory,
            IWheelSpinStrategy randomStrategy)
        {
            _currencyService = new CurrencyService(1000);
            var reviveCommand = new ReviveCommand(_ctx ?? CreateContext(
                zoneService, spinService, rewardService, _currencyService,
                wheelView, hudView, dialogView, buttonView, wheelFactory, randomStrategy));
            var giveUpCommand = new GiveUpCommand(zoneService, rewardService, TransitionTo);

            _ctx = CreateContext(
                zoneService, spinService, rewardService, _currencyService,
                wheelView, hudView, dialogView, buttonView, wheelFactory, randomStrategy);

            _ctx = new GameContext(
                zoneService,
                spinService,
                rewardService,
                _currencyService,
                wheelView,
                hudView,
                dialogView,
                buttonView,
                wheelFactory,
                TransitionTo,
                randomStrategy,
                reviveCommand,
                giveUpCommand,
                _winEffectConfig_value);

            _idleState = new IdleState();
            _spinCommand = new SpinCommand(_idleState, TransitionTo);
            _collectCommand = new CollectCommand(_idleState, TransitionTo);

            _currencyService.OnBalanceChanged += balance => hudView.UpdateCurrencyDisplay(balance);
            hudView.UpdateCurrencyDisplay(_currencyService.GetBalance());

            buttonView.UpdateReviveCost(50);

            TransitionTo(_idleState);
        }

        public void ExecuteSpin() => _spinCommand?.Execute();
        public void ExecuteCollect() => _collectCommand?.Execute();
        public void ExecuteRevive() => _ctx.ReviveCommand?.Execute();

        private GameContext CreateContext(
            IZoneService zoneService,
            ISpinService spinService,
            IRewardService rewardService,
            ICurrencyService currencyService,
            IWheelView wheelView,
            IHudView hudView,
            IDialogView dialogView,
            IButtonView buttonView,
            IWheelFactory wheelFactory,
            IWheelSpinStrategy randomStrategy)
        {
            return new GameContext(
                zoneService, spinService, rewardService, currencyService,
                wheelView, hudView, dialogView, buttonView, wheelFactory,
                TransitionTo, randomStrategy,
                new ReviveCommand(null), new GiveUpCommand(zoneService, rewardService, TransitionTo),
                _winEffectConfig_value);
        }

        private void TransitionTo(IGameState next)
        {
            _currentState?.Exit(_ctx);
            _currentState = next;
            _currentState.Enter(_ctx);
        }

#if UNITY_EDITOR
        public IGameState CurrentStateForTesting => _currentState;
#endif
    }
}