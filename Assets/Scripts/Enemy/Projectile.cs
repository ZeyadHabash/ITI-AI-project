using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private GameObject impactEffect;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        if ((whatIsPlayer.value & (1 << other.gameObject.layer)) == 0)
        {
            if (other.TryGetComponent(out IDamageable damageable)) damageable.TakeDamage(damage);
        }

        Vector3 dir = other.transform.position - transform.position;
        Instantiate(impactEffect, transform.position, Quaternion.LookRotation(gameObject.GetComponent<Rigidbody>().linearVelocity));
        Destroy(gameObject);
    }
}