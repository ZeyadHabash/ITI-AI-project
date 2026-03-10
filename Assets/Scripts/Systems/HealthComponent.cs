using UnityEngine;
using System;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private DamagableType damagableType;
    public DamagableType Type => damagableType;

    public event Action OnTakeDamage;
    private int currentHealth = 100;
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Health: " + currentHealth);
        currentHealth -= damage;

        OnTakeDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

}