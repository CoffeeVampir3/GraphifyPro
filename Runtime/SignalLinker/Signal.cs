using UnityEngine;

namespace Vampire.Runtime.SignalLinker
{
    public abstract class Signal<T> where T : Signal<T> {
        public delegate void EventListener(T info);
        private static event EventListener listeners;

        public static void DebugSignal(Signal<T> sig)
        {
            if (listeners == null) return;
            foreach (var listener in listeners.GetInvocationList())
            {
                Debug.Log(sig.GetType() + " listened to by " + listener.Method.Name);
            }
        }
        
        public static void RegisterListener(EventListener listener) {
            listeners += listener;
        }

        public static void UnregisterListener(EventListener listener) {
            listeners -= listener;
        }

        public void Send()
        {
            listeners?.Invoke(this as T);
        }
    }
}