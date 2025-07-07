using UnityEngine;

public enum Damageable
{
    Player,
    Enemy,
    Environment
}
public class DamageableComponent : MonoBehaviour
{
    public Damageable damageableGroup;
    public HealthComponent health;
}
