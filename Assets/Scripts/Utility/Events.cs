using UnityEngine;

public interface IEvent { }

public struct TestEvent : IEvent { }

public struct LocalPlayerSpawned : IEvent
{
    public PlayerController playerController;
}

public struct PlayerAdded : IEvent
{
    public GameObject playerGameObject;
}
