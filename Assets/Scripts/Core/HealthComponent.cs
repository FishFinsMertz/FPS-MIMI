using UnityEngine;
using UnityEngine.Events;

public struct OnDeath : IEvent {}

public class HealthComponent : MonoBehaviour, IComponent
{
    [Header("Owner Controller Script")]

    [Header("Health Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    // Private or hidden variables
    private LocalEventBusManager localEventBusManager;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(50);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        localEventBusManager.GetLocalEventBus<OnDeath>().Raise(new OnDeath { }, true);
    }

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    public void ResetHealth() => currentHealth = maxHealth;

    // Events
    private void OnEnable()
    {
        if (localEventBusManager != null)
        {
            Initialize(localEventBusManager);
        }
        
        ResetHealth();
    }



    public void Initialize(LocalEventBusManager LEBM)
    {
        localEventBusManager = LEBM;
    }
}
