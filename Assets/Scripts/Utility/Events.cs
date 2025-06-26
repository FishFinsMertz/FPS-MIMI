using UnityEngine;

public interface IEvent { }

public struct TestEvent : IEvent { }

public struct LocalPlayerSpawned : IEvent
{
    public PlayerController playerController;
    public GameObject playerGameObject;
}

public struct PlayerSpawnedEvent : IEvent
{
    public PlayerController playerController;
    public GameObject playerGameObject;
}

