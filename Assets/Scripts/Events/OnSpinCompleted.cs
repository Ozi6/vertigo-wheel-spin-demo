using WheelOfFortune.Domain;

namespace WheelOfFortune.Events
{
    public readonly struct OnSpinCompleted
    {
        public readonly SpinResult Result;

        public OnSpinCompleted(SpinResult result)
        {
            Result = result;
        }
    }
}