using System.Collections;
using Unity.Netcode;
using UnityEngine;

public struct ShootEvent : IEvent 
{
    public GameObject gunOwner;
    public Vector3 bulletOrigin;
    public Vector3 targetDirection;
}

public struct ReloadEvent : IEvent 
{
    public GameObject gunOwner;
}

public struct ShootAfterFXEvent : IEvent 
{
    public GameObject gunOwner;
}

public class GunComponent : NetworkBehaviour, IComponent
{
    EventBinding<ShootEvent> shootEventBinding;
    EventBinding<ReloadEvent> reloadEventBinding;

    [Header("Gun Specs")]
    [SerializeField] int magSize = 6;
    [SerializeField] float reloadTime = 1;
    [SerializeField] BulletSettings bulletType;
    [SerializeField] float shootStrength;
    [SerializeField] int team; //Player team shots dont hit players, enemy team shots dont hit enemies

    [Header("Debug Values")]
    [SerializeField] int ammoCount = -1;
    [SerializeField] int magCount = 6;

    Coroutine reloadCoroutine = null;

    private LocalEventBusManager localEventBusManager;
    private void OnEnable()
    {
        shootEventBinding = new EventBinding<ShootEvent>(Shoot);
        reloadEventBinding = new EventBinding<ReloadEvent>(ReloadEventHandler);
        if (localEventBusManager != null)
        {
            Initialize(localEventBusManager);
        }
    }

    private void OnDisable()
    {
        if (localEventBusManager != null)
        {
            localEventBusManager.GetLocalEventBus<ShootEvent>().Deregister(shootEventBinding);
            localEventBusManager.GetLocalEventBus<ReloadEvent>().Deregister(reloadEventBinding);
        }
    }

    void Shoot(ShootEvent shootEvent) 
    {
        if (shootEvent.gunOwner != gameObject) return;
        if (magCount == 0) return;

        if (magCount != -1) magCount--; //-1 means no reload/Infinite mag size

        SpawnBulletServerRpc(shootEvent.bulletOrigin, shootEvent.targetDirection);
        EventBus<ShootAfterFXEvent>.Raise(new ShootAfterFXEvent { gunOwner = gameObject });
        if (magCount == 0) Reload();
    }

    void Reload() 
    {
        if (reloadCoroutine != null) return;
        if (ammoCount != 0 && magCount != magSize)
        {
            reloadCoroutine = StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine() 
    {
        yield return new WaitForSeconds(reloadTime);
        int prevMagCount = magCount;
        magCount = ammoCount == -1 ? magSize : Mathf.Min(ammoCount, magSize); //Reload magCount to be magSize if infinite ammo, or however many bulllets we can put in
        if (ammoCount != -1) ammoCount -= (magCount - prevMagCount);

        reloadCoroutine = null;
    }

    void ReloadEventHandler(ReloadEvent reloadEvent) 
    {
        if (reloadEvent.gunOwner != gameObject) return;
        Reload();
    }

    public int GetTeam() 
    {
        return team;
    }

    [ServerRpc]
    void SpawnBulletServerRpc(Vector3 bulletOrigin, Vector3 targetDirection) 
    {
        Bullet bullet = BulletFactory.Spawn(bulletType);
        bullet.transform.position = bulletOrigin;
        bullet.team = team;
        Vector3 dir = targetDirection.normalized;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = dir * shootStrength;
        if (!bullet.GetComponent<NetworkObject>().IsSpawned) bullet.GetComponent<NetworkObject>().Spawn();
    }
    public void Initialize(LocalEventBusManager LEBM)
    {
        localEventBusManager = LEBM;
        localEventBusManager.GetLocalEventBus<ShootEvent>().Register(shootEventBinding, true);
        localEventBusManager.GetLocalEventBus<ReloadEvent>().Register(reloadEventBinding, true);
    }
}
