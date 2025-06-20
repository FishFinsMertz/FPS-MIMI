using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private BulletSettings bulletSettings;
    public int team;

    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetBulletSettings(BulletSettings bulletSettings) 
    {
        this.bulletSettings = bulletSettings;
        rb.useGravity = bulletSettings.isAffectedByGravity;
    }
    public BulletSettings GetBulletSettings() => bulletSettings;
    private void OnTriggerEnter(Collider collider)
    {
        GunComponent gun = collider.GetComponentInParent<GunComponent>();
        if (gun && gun.GetTeam() == this.team) return;
        BulletFactory.ReturnToPool(this);
    }
}
