using System;
using UnityEngine;

public class HealthViewModel: ViewModel
{
    public event Action<float> OnHealthChanged;
    public event Action<float> OnMaxHealthChanged;
    private float health;
    private float maxHealth;

    public void SetHealth(float value)
    {
        if (health != value)
        {
            health = value;
            OnHealthChanged?.Invoke(health);
        }
    }

    public void SetMaxHealth(float value)
    {
        if (maxHealth != value)
        {
            maxHealth = value;
            OnMaxHealthChanged?.Invoke(maxHealth);
        }
    }

    public float GetHealth() => health;
    public float GetMaxHealth() => maxHealth;
}
