using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(name: "Is Target Visible", story: "Is [Target] visible within [Range] and [Angle] for [Self]", category: "Conditions", id: "force_vision_condition")]
public partial class IsTargetVisible : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<float> Angle;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        if (Target.Value == null || Self.Value == null) return false;

        Vector3 direction = Target.Value.transform.position - Self.Value.transform.position;

        // 1. Distance Check
        if (direction.magnitude > Range.Value) return false;

        // 2. Angle Check
        float angleToTarget = Vector3.Angle(Self.Value.transform.forward, direction);
        return angleToTarget < (Angle.Value * 0.5f);
    }
}