using UnityEngine;

public class TrollAnimationEvents : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private int attackDamage = 50;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private LayerMask playerLayer;
    public void Attack()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);
        foreach (Collider player in hitPlayers)
        {
            player.GetComponent<IDamageable>().TakeDamage(attackDamage);
        }
    }

}
