using System;
using System.Collections.Generic;

namespace DataLayer
{
    public sealed class ObservableProperty<T>
    {
        private T _value;
        private readonly List<Action> _callbacks = new List<Action>();

        public ObservableProperty(T value = default)
        {
            _value = value;
            NotifyObservers();
        }

        public void Set(T value)
        {
            if (!value.Equals(_value))
            {
                _value = value;

                NotifyObservers();
            }
        }

        public T Get()
        {
            return _value;
        }

        public void Subscribe(Action action, bool invokeActionOnSubscribe = true)
        {
            _callbacks.Add(action);

            if (invokeActionOnSubscribe)
            {
                action?.Invoke();
            }
        }

        public void Unsubscribe(Action action)
        {
            _callbacks.Remove(action);
        }

        private void NotifyObservers()
        {
            foreach (var callback in _callbacks)
            {
                callback?.Invoke();
            }
        }
    }
}
