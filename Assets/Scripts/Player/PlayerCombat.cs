using UnityEngine;


public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    private HealthComponent healthComponent;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
    }

    private void OnEnable()
    {
        if (healthComponent != null)
        {
            healthComponent.OnTakeDamage += PlayHitAnimation;
        }
    }

    private void OnDisable()
    {
        if (healthComponent != null)
        {
            healthComponent.OnTakeDamage -= PlayHitAnimation;
        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SpellAttack();
        }
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

    }
    public void SpellAttack()
    {
        playerAnimator.SetTrigger("Spell");
    }
    public void Attack()
    {
        playerAnimator.SetTrigger("Attack");
    }

    public void PlayHitAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Hit");
        }
    }
}