using System.Collections.Generic;
using UnityEngine;

public class HitBoxComponent : MonoBehaviour
{
    [Header("Damage info")]
    public float Damage;

    [Header("Targets to Damage")]
    // List of groups this hitbox can damage
    public List<Damageable> damageableGroups = new List<Damageable>();

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Hitbox enabled and detects" + other.gameObject.name);
        // Try to get DamageableComponent on hit object or its parent
        DamageableComponent damageableComponent = other.GetComponentInParent<DamageableComponent>();
        if (damageableComponent == null) return; // No component, so no damage
        
        // Check if the target's group is in the list of damageable groups
        if (!damageableGroups.Contains(damageableComponent.damageableGroup)) return;

        // Try to get HealthComponent and apply damage
        HealthComponent health = damageableComponent.health;
        if (health != null)
        {
            health.TakeDamage(Damage);
        }
    }
}
