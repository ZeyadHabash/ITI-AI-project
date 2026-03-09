using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckForTargetAction", story: "Check If [Detector] has a target and set [Target]", category: "Action", id: "6618221502eebecec59ed8f05bdc6675")]
public partial class CheckForTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<CheckTargetInRange> Detector;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!Detector.Value)
            return Status.Failure;

        GameObject detectedTarget = Detector.Value.GetTarget();

        // Target found
        if (detectedTarget)
        {
            if (Target.Value != detectedTarget)
            {
                Target.Value = detectedTarget;
                Debug.Log("Target found: " + Target.Value.name);
            }

            return Status.Running;
        }

        // Target lost
        if (Target.Value != null)
        {
            Target.Value = null;
            Debug.Log("Target lost");
        }

        return Status.Running;
    }
}