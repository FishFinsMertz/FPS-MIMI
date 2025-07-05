using System;
using UnityEngine;
using System.Collections.Generic;

public class EventBus<T> where T : IEvent
{
    static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();
    static List<Action> clearMethods = new List<Action>();
    public static void Register(EventBinding<T> binding) => bindings.Add(binding);
    public static void Deregister(EventBinding<T> binding) => bindings.Remove(binding);

    public static void Raise(T @event) {
        foreach (var binding in bindings) {
            binding.OnEvent.Invoke(@event);
            binding.OnEventNoArgs.Invoke();
        }
    }

    static void Clear() {
        Debug.Log($"Clearing {typeof(T).Name} bindings");
        bindings.Clear();
        foreach(Action method in clearMethods) 
        {
            method();
        }
    }

    public static void addClearMethod(Action clearMethod) 
    {
        clearMethods.Add(clearMethod);
    }
    public static void removeClearMethod(Action clearMethod)
    {
        clearMethods.Remove(clearMethod);
    }
}