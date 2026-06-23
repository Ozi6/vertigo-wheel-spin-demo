using UnityEngine;
using WheelOfFortune.Commands;
using WheelOfFortune.Data;
using WheelOfFortune.Factory;
using WheelOfFortune.Interfaces;
using WheelOfFortune.StateMachine;
using WheelOfFortune.Events;

namespace WheelOfFortune.Controller
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private WinEffectConfig _winEffectConfig_value;

        private GameContext _ctx;
        private IGameState _currentState;
        private IdleState _idleState;
        private ICommand _spinCommand;
        private ICommand _collectCommand;
        private IEventBus _eventBus;

        public void Init(
            IZoneService zoneService,
            ISpinService spinService,
            IRewardService rewardService,
            ICurrencyService currencyService,
            IWheelView wheelView,
            IHudView hudView,
            IDialogView dialogView,
            IButtonView buttonView,
            IWheelFactory wheelFactory,
            ICommandFactory commandFactory,
            IWheelSpinStrategy randomStrategy,
            IWheelSpinStrategy weightedStrategy,
            IRewardRegistry rewardRegistry,
            GameSettingsSO settings,
            IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnStateTransition>(OnStateTransitionRequested);

            _idleState = new IdleState();
            _spinCommand = commandFactory.CreateSpinCommand(_idleState, _eventBus);
            _collectCommand = commandFactory.CreateCollectCommand(_idleState, _eventBus);

            var reviveCmd = commandFactory.CreateReviveCommand(currencyService, _eventBus, settings.StartingReviveCost);
            var giveUpCmd = commandFactory.CreateGiveUpCommand(_eventBus);

            _ctx = new GameContextBuilder()
                .WithServices(zoneService, spinService, rewardService, currencyService, _eventBus)
                .WithViews(wheelView, hudView, dialogView, buttonView)
                .Build(wheelFactory, randomStrategy, weightedStrategy, rewardRegistry, _winEffectConfig_value, reviveCmd, giveUpCmd);

            _eventBus.Publish(new OnBalanceChange(currencyService.GetBalance()));
            _eventBus.Publish(new OnReviveCostChanged(settings.StartingReviveCost));

            buttonView.SetCommands(_spinCommand, _collectCommand);

            TransitionTo(_idleState);
        }

        private void OnStateTransitionRequested(OnStateTransition evt)
        {
            TransitionTo(evt.NewState);
        }

        private void TransitionTo(IGameState next)
        {
            _currentState?.Exit(_ctx);
            _currentState = next;
            _currentState.Enter(_ctx);
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
                _eventBus.Unsubscribe<OnStateTransition>(OnStateTransitionRequested);
        }

#if UNITY_EDITOR
        public IGameState CurrentStateForTesting => _currentState;
#endif
    }
}