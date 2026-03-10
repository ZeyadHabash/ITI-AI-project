using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Summon Exploders", story: "[Self] summons [SummonCount] units of [SummonPrefab] around ring [SummonRingRadius]", category: "Action", id: "a13f7e8d6b5c4a21bf2e0d9149aa1023")]
public partial class SummonExplodersAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> SummonPrefab;
    [SerializeReference] public BlackboardVariable<GameObject> SummonVfxPrefab;
    [SerializeReference] public BlackboardVariable<float> SummonVfxLifetime;
    [SerializeReference] public BlackboardVariable<AudioClip> SummonSfx;
    [SerializeReference] public BlackboardVariable<float> SummonSfxVolume;
    [SerializeReference] public BlackboardVariable<int> SummonCount;
    [SerializeReference] public BlackboardVariable<float> SummonRingRadius;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> SummonWithinDistance;
    [SerializeReference] public BlackboardVariable<float> SummonDelayAfterVfx;
    [SerializeReference] public BlackboardVariable<List<GameObject>> TrackedSummons;
    [SerializeField] private float fallbackSummonDistance = 12f;
    [SerializeField] private float fallbackSummonDelayAfterVfx = 2f;

    private readonly List<Vector3> pendingSpawnPositions = new List<Vector3>();
    private Quaternion pendingRotation;
    private Vector3 pendingCenter;
    private float spawnAtTime;
    private bool waitingToSpawn;

    protected override Status OnStart()
    {
        if (Self.Value == null || SummonPrefab.Value == null)
        {
            return Status.Failure;
        }

        if (TrackedSummons.Value == null)
        {
            TrackedSummons.Value = new List<GameObject>();
        }

        PruneDeadSummons(TrackedSummons.Value);
        if (TrackedSummons.Value.Count > 0)
        {
            return Status.Success;
        }

        int count = Mathf.Max(1, SummonCount.Value);
        float ringRadius = Mathf.Max(0.5f, SummonRingRadius.Value);

        GameObject resolvedTarget = ResolveTarget();
        if (resolvedTarget == null)
        {
            return Status.Failure;
        }

        float configuredDistance = SummonWithinDistance != null ? SummonWithinDistance.Value : 0f;
        float requiredDistance = configuredDistance > 0f ? configuredDistance : Mathf.Max(0.1f, fallbackSummonDistance);
        if (requiredDistance > 0f)
        {
            float distanceToTarget = Vector3.Distance(Self.Value.transform.position, resolvedTarget.transform.position);
            if (distanceToTarget > requiredDistance)
            {
                return Status.Failure;
            }
        }

        pendingCenter = Self.Value.transform.position;
        pendingRotation = Self.Value.transform.rotation;
        pendingSpawnPositions.Clear();

        for (int i = 0; i < count; i++)
        {
            float angle = (Mathf.PI * 2f * i) / count;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * ringRadius;
            Vector3 requestedSpawnPosition = pendingCenter + offset;
            Vector3 spawnPosition = requestedSpawnPosition;
            if (NavMesh.SamplePosition(requestedSpawnPosition, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                spawnPosition = navHit.position;
            }

            pendingSpawnPositions.Add(spawnPosition);

            if (SummonVfxPrefab.Value != null)
            {
                GameObject vfx = UnityEngine.Object.Instantiate(SummonVfxPrefab.Value, spawnPosition, Quaternion.identity);
                if (vfx != null)
                {
                    float lifetime = SummonVfxLifetime.Value > 0f ? SummonVfxLifetime.Value : 3f;
                    UnityEngine.Object.Destroy(vfx, lifetime);
                }
            }
        }

        float delay = SummonDelayAfterVfx != null && SummonDelayAfterVfx.Value >= 0f
            ? SummonDelayAfterVfx.Value
            : fallbackSummonDelayAfterVfx;

        if (delay <= 0f)
        {
            SpawnPendingSummons();
            return Status.Success;
        }

        spawnAtTime = Time.time + delay;
        waitingToSpawn = true;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!waitingToSpawn)
        {
            return Status.Success;
        }

        if (Time.time < spawnAtTime)
        {
            return Status.Running;
        }

        SpawnPendingSummons();
        waitingToSpawn = false;
        return Status.Success;
    }

    protected override void OnEnd()
    {
        if (!waitingToSpawn)
        {
            pendingSpawnPositions.Clear();
        }
    }

    private void SpawnPendingSummons()
    {
        for (int i = 0; i < pendingSpawnPositions.Count; i++)
        {
            Vector3 spawnPosition = pendingSpawnPositions[i];
            GameObject spawned = UnityEngine.Object.Instantiate(SummonPrefab.Value, spawnPosition, pendingRotation);
            if (spawned != null)
            {
                NavMeshAgent spawnedAgent = spawned.GetComponent<NavMeshAgent>();
                if (spawnedAgent != null && NavMesh.SamplePosition(spawned.transform.position, out NavMeshHit warpHit, 1f, NavMesh.AllAreas))
                {
                    spawnedAgent.Warp(warpHit.position);
                }

                TrackedSummons.Value.Add(spawned);
            }
        }

        if (SummonSfx.Value != null)
        {
            float volume = SummonSfxVolume.Value > 0f ? SummonSfxVolume.Value : 1f;
            AudioSource.PlayClipAtPoint(SummonSfx.Value, pendingCenter, volume);
        }

        pendingSpawnPositions.Clear();
        waitingToSpawn = false;
    }

    private static void PruneDeadSummons(List<GameObject> summons)
    {
        for (int i = summons.Count - 1; i >= 0; i--)
        {
            if (summons[i] == null)
            {
                summons.RemoveAt(i);
            }
        }
    }

    private GameObject ResolveTarget()
    {
        if (Target != null && Target.Value != null)
        {
            return Target.Value;
        }

        CheckTargetInRange detector = Self.Value != null ? Self.Value.GetComponent<CheckTargetInRange>() : null;
        return detector != null ? detector.GetTarget() : null;
    }
}
