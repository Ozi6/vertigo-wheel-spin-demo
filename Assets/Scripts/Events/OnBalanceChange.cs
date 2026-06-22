namespace WheelOfFortune.Events
{
    public struct OnBalanceChange
    {
        public int NewBalance { get; }
        public OnBalanceChange(int newBalance) => NewBalance = newBalance;
    }
}