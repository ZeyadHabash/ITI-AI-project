using System.Collections;
using UnityEngine;

/// <summary>
/// Handles triggering the spell animation and spawning the fireball.
/// </summary>
public class SpellCaster : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Animator controlling the Arms. Overrides GetComponent.")]
    [SerializeField] private Animator armsAnimator;

    [Tooltip("The Fireball prefab to instantiate.")]
    [SerializeField] private GameObject fireballPrefab;

    [Tooltip("Transform from where the fireball originates (e.g., the hand).")]
    [SerializeField] private Transform castPoint;

    [Tooltip("The camera used for aiming. Defaults to Camera.main if empty.")]
    [SerializeField] private Camera playerCamera;

    [Header("Settings")]
    [Tooltip("Delay before fireball is spawned to match the animation.")]
    [SerializeField] private float spawnDelay = 0.5f;

    [Tooltip("Maximum aiming distance for the spell.")]
    [SerializeField] private float aimDistance = 100f;

    private static readonly int SpellAnimHash = Animator.StringToHash("Spell");

    private void Awake()
    {
        if (armsAnimator == null)
        {
            armsAnimator = GetComponent<Animator>();
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        // Example input logic
        if (Input.GetMouseButtonDown(1))
        {
            CastSpell();
        }
    }

    /// <summary>
    /// Triggers the animator and schedules the fireball spawn.
    /// </summary>
    public void CastSpell()
    {
        if (armsAnimator != null)
        {
            armsAnimator.SetTrigger(SpellAnimHash);
            StartCoroutine(SpawnFireballRoutine());
        }
    }

    private IEnumerator SpawnFireballRoutine()
    {
        if (spawnDelay > 0f)
        {
            yield return new WaitForSeconds(spawnDelay);
        }

        SpawnFireball();
    }

    /// <summary>
    /// Spawns the fireball aimed at the center of the camera's view.
    /// </summary>
    public void SpawnFireball()
    {
        if (fireballPrefab == null || castPoint == null) return;

        Vector3 direction = GetAimDirection();
        Instantiate(fireballPrefab, castPoint.position, Quaternion.LookRotation(direction));
    }

    /// <summary>
    /// Calculates the aiming direction based on where the camera is looking.
    /// </summary>
    private Vector3 GetAimDirection()
    {
        if (playerCamera == null) return castPoint.forward;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);

        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, aimDistance)
            ? hit.point : ray.GetPoint(aimDistance);

        return (targetPoint - castPoint.position).normalized;
    }
}
