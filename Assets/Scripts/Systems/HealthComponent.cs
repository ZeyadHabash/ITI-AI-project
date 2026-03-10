using System;
using UnityEngine;
using System;
using System.Reflection.Metadata.Ecma335;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private DamagableType damagableType;
    public DamagableType Type => damagableType;
    public static event Action<int> OnHealthChanged;
    public Action OnTakeDamage;
    [SerializeField] private int currentHealth = 100;
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {

        currentHealth -= damage;

        OnTakeDamage?.Invoke();

        if (damagableType == DamagableType.Player) OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    

}