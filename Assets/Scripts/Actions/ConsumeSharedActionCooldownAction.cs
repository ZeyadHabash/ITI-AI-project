using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Consume Shared Action Cooldown", story: "Consume shared cooldown by setting [LastSharedActionTime]", category: "Action", id: "f4b1d3f1dff149f5b5c57ac70398ff2f")]
public partial class ConsumeSharedActionCooldownAction : Action
{
    [SerializeReference] public BlackboardVariable<float> LastSharedActionTime;
    [SerializeReference] public BlackboardVariable<bool> EnableDebugLogs;

    protected override Status OnStart()
    {
        if (LastSharedActionTime == null)
        {
            Debug.LogWarning("[ConsumeSharedActionCooldownAction] LastSharedActionTime is not bound.");
            return Status.Failure;
        }

        LastSharedActionTime.Value = SharedActionCooldownUtility.ConsumeNow();

        bool shouldLog = EnableDebugLogs != null && EnableDebugLogs.Value;
        if (shouldLog)
        {
            Debug.Log($"[ConsumeSharedActionCooldownAction] Shared cooldown consumed at t={LastSharedActionTime.Value:F2}");
        }

        return Status.Success;
    }
}
