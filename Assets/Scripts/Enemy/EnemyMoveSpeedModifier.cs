using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveSpeedModifier : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private float baseSpeed;
    private float activeMultiplier = 1f;
    private float buffEndTime;
    private GameObject auraInstance;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            baseSpeed = navMeshAgent.speed;
        }

        enabled = false;
    }

    public void ApplySpeedBuff(float multiplier, float duration, GameObject auraPrefab)
    {
        if (multiplier <= 1f)
        {
            return;
        }

        if (navMeshAgent != null && activeMultiplier <= 1f)
        {
            baseSpeed = navMeshAgent.speed;
        }

        activeMultiplier = Mathf.Max(activeMultiplier, multiplier);
        buffEndTime = Mathf.Max(buffEndTime, Time.time + Mathf.Max(0.1f, duration));

        if (auraPrefab != null && auraInstance == null)
        {
            auraInstance = Instantiate(auraPrefab, transform);
            auraInstance.transform.localPosition = Vector3.zero;
        }

        ApplyCurrentSpeed();
        enabled = true;
    }

    private void Update()
    {
        if (Time.time >= buffEndTime)
        {
            activeMultiplier = 1f;
            if (navMeshAgent != null)
            {
                navMeshAgent.speed = baseSpeed;
            }

            if (auraInstance != null)
            {
                Destroy(auraInstance);
                auraInstance = null;
            }

            enabled = false;
            return;
        }

        ApplyCurrentSpeed();
    }

    private void ApplyCurrentSpeed()
    {
        if (navMeshAgent == null)
        {
            return;
        }

        navMeshAgent.speed = Mathf.Max(0.01f, baseSpeed * activeMultiplier);
    }
}
