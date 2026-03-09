using UnityEngine;


public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;


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
}