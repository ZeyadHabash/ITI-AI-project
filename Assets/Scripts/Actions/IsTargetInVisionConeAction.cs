using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsTargetInVIsionCone", story: "Is [Target] in vision cone radius [radius] and angle [Angle] from [self]", category: "Action/Conditional", id: "b45d4f93eb28963a2fec0e64f03a4024")]
public partial class IsTargetInVIsionConeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<float> Angle;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (Target.Value == null || Self.Value == null) return Status.Failure;

        Vector3 directionToTarget = Target.Value.transform.position - Self.Value.transform.position;


        if (directionToTarget.magnitude > Radius.Value) return Status.Failure;


        float angleToTarget = Vector3.Angle(Self.Value.transform.forward, directionToTarget);
        if (angleToTarget > Angle.Value / 2f) return Status.Failure;

        return Status.Success;
    }
}

