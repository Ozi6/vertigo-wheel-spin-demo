using System;

namespace WheelOfFortune.Interfaces
{
    public interface IEventBus
    {
        void Publish<T>(T payload);
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
    }
}
