using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveSpeedModifier : MonoBehaviour
{
    [SerializeField] private bool enableDebugLogs = true;

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
            Log($"Initialized with base speed {baseSpeed:F2}");
        }
        else
        {
            Log("No NavMeshAgent found. Buffs will not affect movement speed.");
        }

        enabled = false;
    }

    public void ApplySpeedBuff(float multiplier, float duration, GameObject auraPrefab)
    {
        if (multiplier <= 1f)
        {
            Log($"Ignored buff because multiplier is {multiplier:F2} (must be > 1).");
            return;
        }

        if (navMeshAgent != null && activeMultiplier <= 1f)
        {
            baseSpeed = navMeshAgent.speed;
        }

        activeMultiplier = Mathf.Max(activeMultiplier, multiplier);
        buffEndTime = Mathf.Max(buffEndTime, Time.time + Mathf.Max(0.1f, duration));
        Log($"Buff applied/refreshed. multiplier={activeMultiplier:F2}, endsAt={buffEndTime:F2}");

        if (auraPrefab != null && auraInstance == null)
        {
            auraInstance = Instantiate(auraPrefab, transform);
            auraInstance.transform.localPosition = Vector3.zero;
            Log("Spawned aura VFX instance.");
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
                Log("Destroyed aura VFX instance.");
            }

            Log("Buff expired and speed restored.");
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

    private void Log(string message)
    {
        if (!enableDebugLogs)
        {
            return;
        }

        Debug.Log($"[EnemyMoveSpeedModifier:{name}] {message}");
    }
}
