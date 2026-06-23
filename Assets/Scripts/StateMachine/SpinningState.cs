using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Events;

namespace WheelOfFortune.StateMachine
{
    public sealed class SpinningState : IGameState
    {
        private GameContext _ctx;
        private SpinResult _pendingResult;

        public void Enter(GameContext ctx)
        {
            _ctx = ctx;
            _ctx.EventBus.Subscribe<OnSpinAnimationComplete>(OnSpinAnimationComplete);
            _ctx.ButtonView.SetSpinInteractable(false);
            _ctx.ButtonView.SetCollectVisible(false);

            var zoneType = _ctx.ZoneService.GetCurrentZoneType();
            var zoneNumber = _ctx.ZoneService.GetCurrentZoneNumber();

            var wheelData = _ctx.WheelFactory.BuildWheel(zoneType, zoneNumber, _ctx.WheelView);

            IWheelSpinStrategy strategy = wheelData.IsWeighted
                ? _ctx.WeightedStrategy
                : _ctx.RandomStrategy;

            _ctx.SpinService.SetStrategy(strategy);
            _pendingResult = _ctx.SpinService.Spin(wheelData);

            _ctx.WheelView.SpinTo(_pendingResult.SliceIndex);
        }

        public void Exit(GameContext ctx)
        {
            ctx.EventBus.Unsubscribe<OnSpinAnimationComplete>(OnSpinAnimationComplete);
        }

        private void OnSpinAnimationComplete(OnSpinAnimationComplete evt)
        {
            if (_pendingResult.IsBomb)
                _ctx.EventBus.Publish(new OnStateTransition(new BombState()));
            else
                _ctx.EventBus.Publish(new OnStateTransition(new RewardState(_pendingResult)));
        }
    }
}