using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [Header("Fireball")]
    [SerializeField] private float fireballSpeed = 10f;
    [SerializeField] private Transform castPoint;
    [SerializeField] private Rigidbody fireballPrefab;
    [SerializeField] private Camera playerCamera;

    public void SpawnFireball()
    {
        if (fireballPrefab == null || castPoint == null) return;

        Vector3 direction = GetAimDirection();
        Rigidbody rb = Instantiate(fireballPrefab, castPoint.position, quaternion.identity);

        rb.AddForce(direction * fireballSpeed, ForceMode.Impulse);
        rb.rotation = Quaternion.LookRotation(rb.linearVelocity);
    }
    private Vector3 GetAimDirection()
    {
        if (playerCamera == null) return castPoint.forward;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);

        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, 200)
            ? hit.point : ray.GetPoint(200);

        return (targetPoint - castPoint.position).normalized;
    }
}
