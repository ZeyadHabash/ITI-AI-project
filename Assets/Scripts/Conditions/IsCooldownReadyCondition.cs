using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "Is Cooldown Ready", story: "Is cooldown ready using [LastActionTime] and [CooldownDuration]", category: "Conditions", id: "6f2128b849d14f9fab387f21e73510a5")]
public partial class IsCooldownReadyCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> LastActionTime;
    [SerializeReference] public BlackboardVariable<float> CooldownDuration;
    [SerializeReference] public BlackboardVariable<float> RemainingCooldown;

    public override bool IsTrue()
    {
        return EvaluateCooldown(LastActionTime, CooldownDuration, RemainingCooldown);
    }

    public static bool EvaluateCooldown(
        BlackboardVariable<float> lastActionTime,
        BlackboardVariable<float> cooldownDuration,
        BlackboardVariable<float> remainingCooldown)
    {
        if (lastActionTime == null || cooldownDuration == null)
        {
            if (remainingCooldown != null)
            {
                remainingCooldown.Value = 0f;
            }

            return true;
        }

        float remaining = SharedActionCooldownUtility.GetRemaining(lastActionTime.Value, cooldownDuration.Value);
        if (remainingCooldown != null)
        {
            remainingCooldown.Value = remaining;
        }

        return remaining <= 0f;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}