using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Play Animation", story: "[Self] plays [Trigger]", category: "Action", id: "d1bd31f0472f4d0db1286e9f6315bd34")]
public partial class PlayAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Animator> Animator;
    [SerializeReference] public BlackboardVariable<string> Trigger;

    protected override Status OnStart()
    {
        if (Self?.Value == null)
        {
            return Status.Failure;
        }

        Animator resolvedAnimator = Animator != null ? Animator.Value : null;
        if (resolvedAnimator == null)
        {
            resolvedAnimator = Self.Value.GetComponent<Animator>();
        }

        if (resolvedAnimator == null)
        {
            return Status.Failure;
        }

        string triggerName = Trigger != null ? Trigger.Value : null;
        if (string.IsNullOrWhiteSpace(triggerName))
        {
            return Status.Failure;
        }

        resolvedAnimator.SetTrigger(triggerName);
        return Status.Success;
    }
}
