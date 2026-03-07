using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(name: "Is Target Visible", story: "Is [Target] visible within range [Range] and angle [Angle] for [Self]", category: "Conditions", id: "vision_condition_v5")]
public partial class IsTargetVisible : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<float> Angle;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {

        if (Target.Value == null || Self.Value == null) return false;

        Vector3 directionToTarget = Target.Value.transform.position - Self.Value.transform.position;
        float distance = directionToTarget.magnitude;
        Debug.Log(distance);

        if (distance > Range.Value) return false;


        float angleToTarget = Vector3.Angle(Self.Value.transform.forward, directionToTarget);
        if (angleToTarget > (Angle.Value * 0.5f)) return false;

        return true;
    }
}