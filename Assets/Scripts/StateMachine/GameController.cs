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
        private SpinCommand _spinCommand;
        private CollectCommand _collectCommand;
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
            IWheelSpinStrategy randomStrategy,
            IRewardRegistry rewardRegistry,
            GameSettingsSO settings,
            IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnStateTransition>(OnStateTransitionRequested);

            _idleState = new IdleState();
            var commandFactory = new CommandFactory();
            _spinCommand = commandFactory.CreateSpinCommand(_idleState, _eventBus);

            _ctx = new GameContextBuilder()
                .WithServices(zoneService, spinService, rewardService, currencyService, _eventBus)
                .WithViews(wheelView, hudView, dialogView, buttonView)
                .WithInfrastructure(wheelFactory, randomStrategy, rewardRegistry, _winEffectConfig_value)
                .Build(settings.StartingReviveCost);

            _collectCommand = commandFactory.CreateCollectCommand(_idleState, _eventBus);

            hudView.UpdateCurrencyDisplay(currencyService.GetBalance());

            dialogView.UpdateReviveCost(settings.StartingReviveCost);

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