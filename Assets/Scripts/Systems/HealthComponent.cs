using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private DamagableType damagableType;

    public DamagableType Type => damagableType;

    private int currentHealth = 100;
    private bool isDead;
    private Animator cachedAnimator;
    private int getHitTriggerHash;
    private int dieTriggerHash;
    private bool hasGetHitTrigger;
    private bool hasDieTrigger;

    void Start()
    {
        currentHealth = maxHealth;
        cachedAnimator = GetComponent<Animator>();
        getHitTriggerHash = Animator.StringToHash(getHitTriggerName);
        dieTriggerHash = Animator.StringToHash(dieTriggerName);
        hasGetHitTrigger = HasTriggerParameter(cachedAnimator, getHitTriggerName);
        hasDieTrigger = HasTriggerParameter(cachedAnimator, dieTriggerName);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Health: " + currentHealth);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isDead = true;

            if (cachedAnimator != null && hasDieTrigger)
            {
                cachedAnimator.SetTrigger(dieTriggerHash);
            }

            float destroyDelay = cachedAnimator != null ? Mathf.Max(0f, destroyDelayAfterDeath) : 0f;
            Destroy(gameObject, destroyDelay);
            return;
        }

        if (cachedAnimator != null && hasGetHitTrigger)
        {
            cachedAnimator.SetTrigger(getHitTriggerHash);
        }
    }

    private static bool HasTriggerParameter(Animator animator, string parameterName)
    {
        if (animator == null || string.IsNullOrWhiteSpace(parameterName))
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == parameterName)
            {
                return true;
            }
        }

        return false;
    }
}

