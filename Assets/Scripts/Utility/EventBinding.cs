using System;

internal interface IEventBinding<T>
{
    public Action<T> OnEvent { get; set; }
    public Action OnEventNoArgs { get; set; }
}

public class EventBinding<T> : IEventBinding<T> where T : IEvent
{
    Action<T> OnEvent = _ => { };
    Action OnEventNoArgs = () => { };

    Action<T> IEventBinding<T>.OnEvent
    {
        get => OnEvent;
        set => OnEvent = value;
    }
    Action IEventBinding<T>.OnEventNoArgs
    {
        get => OnEventNoArgs;
        set => OnEventNoArgs = value;
    }

    public EventBinding(Action<T> OnEvent) => this.OnEvent = OnEvent;
    public EventBinding(Action OnEventNoArgs) => this.OnEventNoArgs = OnEventNoArgs;

    public void Add(Action<T> OnEvent) => this.OnEvent += OnEvent;
    public void Add(Action OnEventNoArgs) => this.OnEventNoArgs += OnEventNoArgs;

    public void Remove(Action<T> OnEvent) => this.OnEvent -= OnEvent;
    public void Remove(Action OnEventNoArgs) => this.OnEventNoArgs -= OnEventNoArgs;
}