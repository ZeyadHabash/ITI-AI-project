using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "Is Shared Action Cooldown Ready", story: "Is shared action cooldown ready using [LastSharedActionTime] and [SharedActionCooldown]", category: "Conditions", id: "db6a06f35f8744758e8745c11b8d1c20")]
public partial class IsSharedActionCooldownReadyCondition : IsCooldownReadyCondition
{
    [SerializeReference] public BlackboardVariable<float> LastSharedActionTime;
    [SerializeReference] public BlackboardVariable<float> SharedActionCooldown;
    [SerializeReference] public BlackboardVariable<float> RemainingSharedCooldown;

    public override bool IsTrue()
    {
        return EvaluateCooldown(LastSharedActionTime, SharedActionCooldown, RemainingSharedCooldown);
    }
}
