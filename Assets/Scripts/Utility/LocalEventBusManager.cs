using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalEventBusManager
{
    private Dictionary<Type, LocalEventBusBase> eventBusTypes = new ();
    public LocalEventBus<T> GetLocalEventBus<T>() where T : IEvent 
    {
        Type type = typeof(T);
        if (eventBusTypes.ContainsKey(type)) return (LocalEventBus<T>)eventBusTypes[type];
        LocalEventBus<T> bus = new LocalEventBus<T>();
        eventBusTypes.Add(type, bus);
        bus.Initialize();
        return bus;
    }
    private void Uninitialize()
    {
        foreach (var type in eventBusTypes.Keys) 
        {
            eventBusTypes[type].Uninitialize();
        }
        eventBusTypes.Clear();
    }
}
