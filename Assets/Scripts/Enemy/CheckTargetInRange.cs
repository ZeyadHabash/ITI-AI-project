using UnityEngine;

public class CheckTargetInRange : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float radius = 5f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float checkDelay = 0.2f;
    [SerializeField] private string preferredTargetTag = "Player";
    [SerializeField] private bool allowFallbackTargets = false;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private Collider[] results = new Collider[32];
    private GameObject currentTarget;

    private void Start()
    {
        InvokeRepeating(nameof(CheckTarget), 0f, checkDelay);
    }

    private void CheckTarget()
    {
        currentTarget = null;

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            radius,
            results,
            targetLayer
        );

        if (count <= 0)
        {
            return;
        }

        GameObject closestPreferred = null;
        float preferredDistanceSqr = float.MaxValue;
        GameObject closestFallback = null;
        float fallbackDistanceSqr = float.MaxValue;

        Vector3 origin = transform.position;
        for (int i = 0; i < count; i++)
        {
            Collider hit = results[i];
            if (hit == null)
            {
                continue;
            }

            Transform hitRoot = hit.transform.root;
            if (hitRoot == transform.root)
            {
                continue;
            }

            GameObject candidate = hitRoot.gameObject;
            float distSqr = (candidate.transform.position - origin).sqrMagnitude;

            bool isPreferred = !string.IsNullOrEmpty(preferredTargetTag) && candidate.CompareTag(preferredTargetTag);
            if (isPreferred)
            {
                if (distSqr < preferredDistanceSqr)
                {
                    preferredDistanceSqr = distSqr;
                    closestPreferred = candidate;
                }
            }
            else if (distSqr < fallbackDistanceSqr)
            {
                fallbackDistanceSqr = distSqr;
                closestFallback = candidate;
            }
        }

        if (closestPreferred != null)
        {
            currentTarget = closestPreferred;
            return;
        }

        currentTarget = allowFallbackTargets ? closestFallback : null;
    }

    public GameObject GetTarget()
    {
        return currentTarget;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}