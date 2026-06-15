using System;
using UnityEngine;
using WheelOfFortune.Commands;
using WheelOfFortune.Data;
using WheelOfFortune.Factory;
using WheelOfFortune.Interfaces;
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
            GameSettingsSO settings)
        {
            _idleState = new IdleState();

            var commandFactory = new CommandFactory();
            _spinCommand = commandFactory.CreateSpinCommand(_idleState, TransitionTo);
            _collectCommand = commandFactory.CreateCollectCommand(_idleState, TransitionTo);

            var contextBuilder = new GameContextBuilder();
            _ctx = contextBuilder
                .WithServices(zoneService, spinService, rewardService, currencyService)
                .WithViews(wheelView, hudView, dialogView, buttonView)
                .WithInfrastructure(wheelFactory, TransitionTo, randomStrategy, _winEffectConfig_value)
                .Build(
                    reviveFactory: ctx => commandFactory.CreateReviveCommand(ctx, settings.StartingReviveCost),
                    giveUpFactory: ctx => commandFactory.CreateGiveUpCommand(zoneService, rewardService, TransitionTo)
                );

            currencyService.OnBalanceChanged += balance => hudView.UpdateCurrencyDisplay(balance);
            hudView.UpdateCurrencyDisplay(currencyService.GetBalance());

            buttonView.UpdateReviveCost(settings.StartingReviveCost);

            TransitionTo(_idleState);
        }

        public void ExecuteSpin() => _spinCommand?.Execute();
        public void ExecuteCollect() => _collectCommand?.Execute();
        public void ExecuteRevive() => _ctx.ReviveCommand?.Execute();

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