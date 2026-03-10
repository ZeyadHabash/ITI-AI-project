using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Apply Move Speed Buff Aura", story: "[Self] buffs nearby enemies by [BuffMultiplier]x in [BuffRadius]", category: "Action", id: "7d92c4ea8a1242d0b1f0d9ce652fd841")]
public partial class ApplyMoveSpeedBuffAuraAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> BuffRadius;
    [SerializeReference] public BlackboardVariable<float> BuffMultiplier;
    [SerializeReference] public BlackboardVariable<float> BuffDuration;
    [SerializeReference] public BlackboardVariable<float> LastBuffTime;
    [SerializeReference] public BlackboardVariable<GameObject> BuffAuraVfxPrefab;
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool matchEnemyTag = true;
    [SerializeField] private bool matchEnemyLayer = true;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string enemyLayerName = "Enemy";

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Log("Failed: Self is null.");
            return Status.Failure;
        }

        float radius = Mathf.Max(0.1f, BuffRadius.Value);
        float multiplier = Mathf.Max(1f, BuffMultiplier.Value);
        float duration = Mathf.Max(0.1f, BuffDuration.Value);
        Log($"Running aura tick from '{Self.Value.name}'. radius={radius:F2}, multiplier={multiplier:F2}, duration={duration:F2}s");

        Collider[] hits = Physics.OverlapSphere(Self.Value.transform.position, radius);
        HashSet<GameObject> processedTargets = new HashSet<GameObject>();
        bool appliedAnyBuff = false;
        int consideredCount = 0;

        foreach (Collider hit in hits)
        {
            GameObject target = ResolveBuffTarget(hit);
            if (target == null)
            {
                continue;
            }

            consideredCount++;
            if (!processedTargets.Add(target))
            {
                continue;
            }

            EnemyMoveSpeedModifier modifier = target.GetComponent<EnemyMoveSpeedModifier>();
            if (modifier == null)
            {
                modifier = target.AddComponent<EnemyMoveSpeedModifier>();
            }

            modifier.ApplySpeedBuff(multiplier, duration, BuffAuraVfxPrefab.Value);
            Log($"Applied buff to '{target.name}'");
            appliedAnyBuff = true;
        }

        Log($"Aura tick complete. colliders={hits.Length}, considered={consideredCount}, uniqueBuffTargets={processedTargets.Count}, appliedAny={appliedAnyBuff}");

        LastBuffTime.Value = Time.time;
        return Status.Success;
    }

    private GameObject ResolveBuffTarget(Collider hit)
    {
        if (hit == null)
        {
            return null;
        }

        Transform selfRoot = Self.Value != null ? Self.Value.transform.root : null;
        Transform hitRoot = hit.transform.root;
        if (selfRoot != null && hitRoot == selfRoot)
        {
            return null;
        }

        NavMeshAgent navAgent = hit.GetComponentInParent<NavMeshAgent>();
        if (IsEnemyTarget(navAgent != null ? navAgent.gameObject : null, selfRoot))
        {
            return navAgent.gameObject;
        }

        EnemyMoveSpeedModifier existingModifier = hit.GetComponentInParent<EnemyMoveSpeedModifier>();
        if (IsEnemyTarget(existingModifier != null ? existingModifier.gameObject : null, selfRoot))
        {
            return existingModifier.gameObject;
        }

        HealthComponent health = hit.GetComponentInParent<HealthComponent>();
        if (IsEnemyTarget(health != null ? health.gameObject : null, selfRoot))
        {
            return health.gameObject;
        }

        Component[] components = hit.GetComponentsInParent<Component>(true);
        for (int i = 0; i < components.Length; i++)
        {
            Component component = components[i];
            if (component is IEnemy)
            {
                if (IsEnemyTarget(component.gameObject, selfRoot))
                {
                    return component.gameObject;
                }
            }
        }

        return null;
    }

    private bool IsEnemyTarget(GameObject candidate, Transform selfRoot)
    {
        if (candidate == null)
        {
            return false;
        }

        Transform candidateRoot = candidate.transform.root;
        if (selfRoot != null && candidateRoot == selfRoot)
        {
            return false;
        }

        GameObject rootObject = candidateRoot != null ? candidateRoot.gameObject : candidate;

        bool layerMatch = false;
        if (matchEnemyLayer && !string.IsNullOrWhiteSpace(enemyLayerName))
        {
            int enemyLayer = LayerMask.NameToLayer(enemyLayerName);
            if (enemyLayer >= 0)
            {
                layerMatch = rootObject.layer == enemyLayer || candidate.layer == enemyLayer;
            }
        }

        bool tagMatch = false;
        if (matchEnemyTag && !string.IsNullOrWhiteSpace(enemyTag))
        {
            tagMatch = SafeCompareTag(rootObject, enemyTag) || SafeCompareTag(candidate, enemyTag);
        }

        return layerMatch || tagMatch;
    }

    private static bool SafeCompareTag(GameObject gameObject, string tagName)
    {
        if (gameObject == null || string.IsNullOrWhiteSpace(tagName))
        {
            return false;
        }

        try
        {
            return gameObject.CompareTag(tagName);
        }
        catch (UnityException)
        {
            return false;
        }
    }

    private void Log(string message)
    {
        if (!enableDebugLogs)
        {
            return;
        }

        Debug.Log($"[ApplyMoveSpeedBuffAuraAction] {message}");
    }
}
