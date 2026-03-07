using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsTargetInVisionCone", story: "Is [Target] inside vision range [visionRange] and angle [visionAngle]", category: "Action/Conditional", id: "ff4448be221825d9d14cd70a36af94fa")]
public partial class IsTargetInVisionConeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> VisionRange;
    [SerializeReference] public BlackboardVariable<float> VisionAngle;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

