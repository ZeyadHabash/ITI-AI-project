using UnityEngine;

public class CheckTargetInRange : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float radius = 5f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float checkDelay = 0.2f;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private Collider[] results = new Collider[1];
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

        if (count > 0 && results[0] != null)
        {
            currentTarget = results[0].gameObject;
        }
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