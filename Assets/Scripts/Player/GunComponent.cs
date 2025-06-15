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

public struct SpawnBulletInformation 
{
    public Vector3 bulletOrigin;
    public Vector3 targetDirection;
}
public class GunComponent : NetworkBehaviour
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

    private void OnEnable()
    {
        shootEventBinding = new EventBinding<ShootEvent>(Shoot);
        EventBus<ShootEvent>.Register(shootEventBinding);

        reloadEventBinding = new EventBinding<ReloadEvent>(ReloadEventHandler);
        EventBus<ReloadEvent>.Register(reloadEventBinding);
    }

    private void OnDisable()
    {
        EventBus<ShootEvent>.Deregister(shootEventBinding);
    }

    void Shoot(ShootEvent shootEvent) 
    {
        if (shootEvent.gunOwner != gameObject) return;
        if (magCount == 0) return;

        if (magCount != -1) magCount--; //-1 means no reload/Infinite mag size

        SpawnBulletServerRpc(shootEvent.bulletOrigin, shootEvent.targetDirection);

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
    }
}
