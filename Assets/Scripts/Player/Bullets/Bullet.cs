using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

interface IShootable {
    public void Shoot(Vector3 hitPosition, Vector3 velocity, float mass);
}
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

        // Bullet damage logic on enemy
        if (IsServer)
        {
            // Try to get HealthComponent on hit object or its parent
            HealthComponent health = collider.GetComponentInParent<HealthComponent>();
            if (health != null)
            {
                health.TakeDamage(bulletSettings.damage); // Add damageAmount field to BulletSettings
            }
        }

        if (collider.TryGetComponent(out IShootable shootableObj)) 
        {
            shootableObj.Shoot(transform.position, rb.linearVelocity, 1);
        }
        BulletFactory.ReturnToPool(this);
    }
}
