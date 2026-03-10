using UnityEngine.UI;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Image healthBar;



    void Start()
    {
        healthBar = GameObject.FindGameObjectsWithTag("HealthBar")[0].GetComponent<Image>();
        HealthComponent.OnHealthChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar(int health)
    {
        if (healthBar == null)
        {
            healthBar = GameObject.FindGameObjectsWithTag("HealthBar")[0].GetComponent<Image>();
        }

        healthBar.fillAmount = health / 100f;
    }
}
