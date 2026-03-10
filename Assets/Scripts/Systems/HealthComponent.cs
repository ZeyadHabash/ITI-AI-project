using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private DamagableType damagableType;

    public DamagableType Type => damagableType;
    public static event Action<int> OnHealthChanged;
    [SerializeField] private int currentHealth = 100;
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
        if (damagableType == DamagableType.Player) OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

}