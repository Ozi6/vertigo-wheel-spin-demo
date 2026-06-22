namespace WheelOfFortune.Utility
{
    public static class MultiplierFormatter
    {
        public static string Format(int multiplier) => $"x{multiplier}";

        public static string Format(float multiplier) => $"x{multiplier}";
    }
}