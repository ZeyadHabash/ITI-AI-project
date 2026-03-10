using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Apply Move Speed Buff Aura", story: "[Self] buffs nearby enemies by [BuffMultiplier]x in [BuffRadius]", category: "Action", id: "7d92c4ea8a1242d0b1f0d9ce652fd841")]
public partial class ApplyMoveSpeedBuffAuraAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Animator> Animator;
    [SerializeReference] public BlackboardVariable<string> BuffTrigger;
    [SerializeReference] public BlackboardVariable<float> BuffRadius;
    [SerializeReference] public BlackboardVariable<float> BuffMultiplier;
    [SerializeReference] public BlackboardVariable<float> BuffDuration;
    [SerializeReference] public BlackboardVariable<float> BuffReapplyInterval;
    [SerializeReference] public BlackboardVariable<float> LastBuffTime;
    [SerializeReference] public BlackboardVariable<GameObject> BuffAuraVfxPrefab;
    [SerializeField] private string fallbackBuffTrigger = "Buff";

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            return Status.Failure;
        }

        float reapplyInterval = Mathf.Max(0f, BuffReapplyInterval.Value);
        if (Time.time < LastBuffTime.Value + reapplyInterval)
        {
            return Status.Success;
        }

        float radius = Mathf.Max(0.1f, BuffRadius.Value);
        float multiplier = Mathf.Max(1f, BuffMultiplier.Value);
        float duration = Mathf.Max(0.1f, BuffDuration.Value);

        Collider[] hits = Physics.OverlapSphere(Self.Value.transform.position, radius);
        HashSet<GameObject> processedTargets = new HashSet<GameObject>();
        bool appliedAnyBuff = false;

        foreach (Collider hit in hits)
        {
            HealthComponent health = hit.GetComponentInParent<HealthComponent>();
            if (health == null)
            {
                continue;
            }

            GameObject target = health.gameObject;
            if (target == Self.Value || !processedTargets.Add(target))
            {
                continue;
            }

            EnemyMoveSpeedModifier modifier = target.GetComponent<EnemyMoveSpeedModifier>();
            if (modifier == null)
            {
                modifier = target.AddComponent<EnemyMoveSpeedModifier>();
            }

            modifier.ApplySpeedBuff(multiplier, duration, BuffAuraVfxPrefab.Value);
            appliedAnyBuff = true;
        }

        if (appliedAnyBuff)
        {
            Animator resolvedAnimator = Animator != null ? Animator.Value : null;
            if (resolvedAnimator == null && Self.Value != null)
            {
                resolvedAnimator = Self.Value.GetComponent<Animator>();
            }

            string triggerName = BuffTrigger != null && !string.IsNullOrWhiteSpace(BuffTrigger.Value)
                ? BuffTrigger.Value
                : fallbackBuffTrigger;

            if (resolvedAnimator != null && !string.IsNullOrWhiteSpace(triggerName))
            {
                resolvedAnimator.SetTrigger(triggerName);
            }
        }

        LastBuffTime.Value = Time.time;
        return Status.Success;
    }
}
