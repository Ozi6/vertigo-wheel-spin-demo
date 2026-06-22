using System;
using System.Collections.Generic;
using UnityEngine;

namespace WheelOfFortune.Utility
{
    public sealed class ComponentPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly Stack<T> _inactive = new Stack<T>();
        private readonly HashSet<T> _active = new HashSet<T>();

        public ComponentPool(T prefab, string containerName = null, int prewarm = 0, Action<T> onGet = null, Action<T> onRelease = null)
        {
            _prefab = prefab;
            _onGet = onGet;
            _onRelease = onRelease;

            var containerGo = new GameObject(containerName ?? $"Pool_{typeof(T).Name}");
            _container = containerGo.transform;
            _container.gameObject.SetActive(false);

            for (int i = 0; i < prewarm; i++)
                _inactive.Push(CreateNew());
        }

        public int ActiveCount => _active.Count;
        public int InactiveCount => _inactive.Count;

        public T Get(Transform parent)
        {
            var instance = _inactive.Count > 0 ? _inactive.Pop() : CreateNew();
            instance.transform.SetParent(parent, false);
            instance.gameObject.SetActive(true);
            _active.Add(instance);
            _onGet?.Invoke(instance);
            return instance;
        }

        public void Release(T instance)
        {
            if (instance == null || !_active.Remove(instance)) return;
            _onRelease?.Invoke(instance);
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_container, false);
            _inactive.Push(instance);
        }

        public void ReleaseAll()
        {
            if (_active.Count == 0) return;
            var snapshot = new List<T>(_active);
            foreach (var instance in snapshot)
                Release(instance);
        }

        public void Clear()
        {
            foreach (var instance in _active)
                if (instance != null) UnityEngine.Object.Destroy(instance.gameObject);
            _active.Clear();

            foreach (var instance in _inactive)
                if (instance != null) UnityEngine.Object.Destroy(instance.gameObject);
            _inactive.Clear();

            if (_container != null) UnityEngine.Object.Destroy(_container.gameObject);
        }

        private T CreateNew()
        {
            return UnityEngine.Object.Instantiate(_prefab, _container);
        }
    }
}