using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Events")]
    public UnityEvent onDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
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
        onDeath?.Invoke();
    }

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    public void ResetHealth() => currentHealth = maxHealth;
}
