using UnityEngine;

/// <summary>
/// // Destroys the object after a set amount of time
/// // Attached to all effects
/// </summary>
public class EffectScript : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;
    void Start() => Destroy(gameObject, lifeTime);

}