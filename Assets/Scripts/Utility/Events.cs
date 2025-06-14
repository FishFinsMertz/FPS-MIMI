public interface IEvent { }

public struct TestEvent : IEvent { }

public struct  LocalPlayerSpawned : IEvent
{
    public PlayerController playerController;
}
