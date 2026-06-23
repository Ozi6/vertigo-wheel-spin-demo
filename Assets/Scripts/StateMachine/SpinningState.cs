using WheelOfFortune.Domain;
using WheelOfFortune.Interfaces;
using WheelOfFortune.Services;

namespace WheelOfFortune.StateMachine
{
    public sealed class SpinningState : IGameState
    {
        private GameContext _ctx;
        private SpinResult _pendingResult;

        public void Enter(GameContext ctx)
        {
            _ctx = ctx;
            _ctx.ButtonView.SetSpinInteractable(false);
            _ctx.ButtonView.SetCollectVisible(false);

            var zoneType = _ctx.ZoneService.GetCurrentZoneType();
            var zoneNumber = _ctx.ZoneService.GetCurrentZoneNumber();

            var wheelData = _ctx.WheelFactory.BuildWheel(zoneType, zoneNumber, _ctx.WheelView);

            IWheelSpinStrategy strategy = wheelData.IsWeighted
                ? new WeightedSpinStrategy()
                : _ctx.RandomStrategy;

            _ctx.SpinService.SetStrategy(strategy);
            _pendingResult = _ctx.SpinService.Spin(wheelData);

            _ctx.WheelView.SpinTo(_pendingResult.SliceIndex, OnSpinAnimationComplete);
        }

        public void Exit(GameContext ctx) { }

        private void OnSpinAnimationComplete()
        {
            if (_pendingResult.IsBomb)
                _ctx.EventBus.Publish(new Events.OnStateTransition(new BombState()));
            else
                _ctx.EventBus.Publish(new Events.OnStateTransition(new RewardState(_pendingResult)));
        }
    }
}
