using UnityEngine;

/// <summary>
/// // Destroys the object after a set amount of time
/// // Attached to all effects
/// </summary>
public class EffectScript : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private AudioSource audioSource;
    void Start()
    {
        Destroy(gameObject, lifeTime);

        if (impactSound != null && audioSource != null) audioSource.PlayOneShot(impactSound);
    }


}