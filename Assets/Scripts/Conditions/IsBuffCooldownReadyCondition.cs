using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "Is Buff Cooldown Ready", story: "Is buff cooldown ready using [LastBuffTime] and [BuffReapplyInterval]", category: "Conditions", id: "c1f877bcf3b644eb9ab8d3c93a9f3de1")]
public partial class IsBuffCooldownReadyCondition : IsCooldownReadyCondition
{
    [SerializeReference] public BlackboardVariable<float> LastBuffTime;
    [SerializeReference] public BlackboardVariable<float> BuffReapplyInterval;
    [SerializeReference] public BlackboardVariable<float> RemainingBuffCooldown;

    public override bool IsTrue()
    {
        return EvaluateCooldown(LastBuffTime, BuffReapplyInterval, RemainingBuffCooldown);
    }
}