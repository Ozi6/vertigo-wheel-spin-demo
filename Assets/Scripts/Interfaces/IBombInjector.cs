using WheelOfFortune.Data;

namespace WheelOfFortune.Interfaces
{
    public interface IBombInjector
    {
        int InjectBomb(SliceDefinition[] slices);
    }
}