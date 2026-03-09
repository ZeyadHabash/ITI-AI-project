using UnityEngine;

/// <summary>
/// Simple script to move the fireball forward and optionally destroy it on impact.
/// Attach this to your Fireball prefab.
/// </summary>
public class FireballProjectile : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Speed in meters per second.")]
    [SerializeField] private float travelSpeed = 20f;

    [Tooltip("How long before the fireball is automatically destroyed.")]
    [SerializeField] private float lifetime = 5f;

    [Header("Effects")]
    [Tooltip("The explosion particle prefab to spawn on impact.")]
    [SerializeField] private GameObject explosionPrefab;

    private void Start()
    {
        // Automatically destroy after lifetime to avoid clutter
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        MoveForward();
    }

    /// <summary>
    /// Moves the object forward according to its local rotation.
    /// </summary>
    private void MoveForward()
    {
        transform.Translate(Vector3.forward * (travelSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Fireball hit: " + other.gameObject.name);
        // Spawn explosion particle if provided
        if (explosionPrefab != null)
        {
            // Spawn at the fireball's current position and rotation
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        // Destroy the fireball when it hits something
        Destroy(gameObject);
    }
}
