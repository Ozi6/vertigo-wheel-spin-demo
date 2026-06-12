using System;
using System.Collections.Generic;
using WheelOfFortune.Interfaces;

namespace WheelOfFortune.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        public void Publish<T>(T payload)
        {
            if (_handlers.TryGetValue(typeof(T), out var existing))
                (existing as Action<T>)?.Invoke(payload);
        }

        public void Subscribe<T>(Action<T> handler)
        {
            var key = typeof(T);
            if (_handlers.TryGetValue(key, out var existing))
                _handlers[key] = (Action<T>)existing + handler;
            else
                _handlers[key] = handler;
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var key = typeof(T);
            if (!_handlers.TryGetValue(key, out var existing)) return;

            var updated = (Action<T>)existing - handler;
            if (updated == null)
                _handlers.Remove(key);
            else
                _handlers[key] = updated;
        }
    }
}
