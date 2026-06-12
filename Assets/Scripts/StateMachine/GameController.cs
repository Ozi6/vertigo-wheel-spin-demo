using UnityEngine;
using WheelOfFortune.Commands;
using WheelOfFortune.Data;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.StateMachine
{
    public sealed class GameController : MonoBehaviour
    {
        private GameContext _ctx;
        private IGameState _currentState;

        private IdleState _idleState;
        private SpinCommand _spinCommand;
        private CollectCommand _collectCommand;

        public void Init(
            IZoneService zoneService,
            ISpinService spinService,
            IRewardService rewardService,
            IWheelView wheelView,
            IHudView hudView,
            IDialogView dialogView,
            ZoneConfigSO[] zoneConfigs,
            IWheelSpinStrategy randomStrategy)
        {
            _ctx = new GameContext(
                zoneService,
                spinService,
                rewardService,
                wheelView,
                hudView,
                dialogView,
                zoneConfigs,
                TransitionTo,
                randomStrategy);

            _idleState = new IdleState();
            _spinCommand = new SpinCommand(_idleState, TransitionTo);
            _collectCommand = new CollectCommand(_idleState, TransitionTo);

            TransitionTo(_idleState);
        }

        public void ExecuteSpin() => _spinCommand?.Execute();

        public void ExecuteCollect() => _collectCommand?.Execute();

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