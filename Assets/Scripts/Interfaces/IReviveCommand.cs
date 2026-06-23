namespace WheelOfFortune.Interfaces
{
    public interface IReviveCommand : ICommand
    {
        int CurrentCost { get; }
        void Reset();
    }
}