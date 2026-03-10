using UnityEngine.UI;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Image healthBar;

    void Start()
    {
        HealthComponent.OnHealthChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar(int health)
    {
        healthBar.fillAmount = health / 100f;
    }
}
