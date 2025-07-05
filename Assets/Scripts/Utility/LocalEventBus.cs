using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

public class LocalEventBusBase 
{
    public virtual void Initialize(){}
    public virtual void Uninitialize(){}
}
public class LocalEventBus<T> : LocalEventBusBase where T : IEvent
{
    readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();
    HashSet<EventBinding<T>> globalBindings = new HashSet<EventBinding<T>>();

    public void Register(EventBinding<T> binding, bool local = false)
    {
        bindings.Add(binding);
        if (!local)
        {
            globalBindings.Add(binding);
            EventBus<T>.Register(binding);
        }
    }
    public void Deregister(EventBinding<T> binding) 
    {
        bindings.Remove(binding);
        if (globalBindings.Contains(binding)) 
        {
            globalBindings.Remove(binding);
            EventBus<T>.Deregister(binding);
        }
    }

    public void Raise(T @event, bool local = false)
    {
        if (local)
        {
            foreach (var binding in bindings)
            {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }
        else
        {
            EventBus<T>.Raise(@event);
        }
    }

    void Clear()
    {
        Debug.Log($"Clearing local {typeof(T).Name} bindings");
        foreach (var binding in globalBindings)
        {
            EventBus<T>.Deregister(binding);
        }
        globalBindings.Clear();
        bindings.Clear();
    }

    public override void Initialize()
    {
        EventBus<T>.addClearMethod(Clear);
    }
    public override void Uninitialize()
    {
        EventBus<T>.removeClearMethod(Clear);
        Clear();
    }
}