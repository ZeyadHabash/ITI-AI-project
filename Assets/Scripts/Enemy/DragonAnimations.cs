using UnityEngine;

public class DragonAnimations : MonoBehaviour
{


    [SerializeField] private Rigidbody fireball;
    [SerializeField] private Transform fireballSpawnPoint;
    [SerializeField] private float fireballSpeed = 10f;
    private GameObject target;

    private void Start()
    {
        target = GameObject.Find("Player");
    }

    public void Attack()
    {

        Rigidbody fireballInstance = Instantiate(fireball, fireballSpawnPoint.position, fireballSpawnPoint.rotation);
        fireballInstance.linearVelocity = (target.transform.position - fireballSpawnPoint.position).normalized * fireballSpeed;
    }
}
