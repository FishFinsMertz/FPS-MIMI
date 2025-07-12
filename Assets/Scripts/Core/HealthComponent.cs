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
    public HealthViewModel healthViewModel;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public ViewModel CreateAndBindViewModel()
    {
        healthViewModel = new HealthViewModel();
        healthViewModel.SetHealth(currentHealth);
        healthViewModel.SetMaxHealth(maxHealth);
        return healthViewModel;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f); // Prevent health from going below 0
        healthViewModel?.SetHealth(currentHealth);

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
        //Debug.Log($"{gameObject.name} died.");
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
