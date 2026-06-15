namespace WheelOfFortune.Interfaces
{
    public interface IGameStateGuard
    {
        bool CanSpin();
        bool CanCollect();
    }
}