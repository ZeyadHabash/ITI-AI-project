using System;
using UnityEngine;
using System;
using System.Reflection.Metadata.Ecma335;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private DamagableType damagableType;
    [SerializeField] private bool destroyOnDeath = true;

    public DamagableType Type => damagableType;
    public bool IsDead => isDead;

    public event Action<HealthComponent> OnDied;
    public static event Action<int> OnHealthChanged;
    public Action OnTakeDamage;
    [SerializeField] private int currentHealth = 100;
    private bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        Debug.Log("Health: " + currentHealth);
        currentHealth -= damage;

        OnTakeDamage?.Invoke();

        if (damagableType == DamagableType.Player) OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        OnDied?.Invoke(this);

        bool shouldDestroy = destroyOnDeath && damagableType != DamagableType.Player;
        if (shouldDestroy)
        {
            Destroy(gameObject);
        }
    }

    

}