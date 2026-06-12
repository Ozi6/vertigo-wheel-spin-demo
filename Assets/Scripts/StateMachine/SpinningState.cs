using WheelOfFortune.Data;
using WheelOfFortune.Domain;

namespace WheelOfFortune.StateMachine
{
    public sealed class SpinningState : IGameState
    {
        private SpinResult _pendingResult;

        public void Enter(GameContext ctx)
        {
            var zoneType = ctx.ZoneService.GetCurrentZoneType();
            var config = GetConfigForZone(ctx, zoneType);
            var strategy = zoneType == ZoneType.Super ? ctx.WeightedStrategy : ctx.RandomStrategy;

            ctx.SpinService.SetStrategy(strategy);
            _pendingResult = ctx.SpinService.Spin(config);

            ctx.WheelView.SpinTo(_pendingResult.SliceIndex, () => OnSpinAnimationComplete(ctx));
        }

        public void Exit(GameContext ctx) { }

        private void OnSpinAnimationComplete(GameContext ctx)
        {
            if (_pendingResult.IsBomb)
                ctx.TransitionTo(new BombState(ctx.ReviveCommand, ctx.GiveUpCommand));
            else
                ctx.TransitionTo(new RewardState(_pendingResult));
        }

        private WheelConfigSO GetConfigForZone(GameContext ctx, ZoneType zoneType)
        {
            if (ctx.ZoneConfigs == null || ctx.ZoneConfigs.Length == 0)
                return null;

            foreach (var zoneConfig in ctx.ZoneConfigs)
            {
                if (zoneConfig.ZoneType == zoneType)
                    return zoneConfig.WheelConfig;
            }

            return ctx.ZoneConfigs[0].WheelConfig;
        }
    }
}