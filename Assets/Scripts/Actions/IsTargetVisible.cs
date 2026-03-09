using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(name: "Is Target Visible", story: "Is [Target] visible within range [Range] from [Self] is [Found]", category: "Conditions", id: "vision_condition_v5")]
public partial class IsTargetVisible : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> Found;

    public override bool IsTrue()
    {

        if (Target.Value == null || Self.Value == null) return false;

        Vector3 directionToTarget = Target.Value.transform.position - Self.Value.transform.position;
        float distance = directionToTarget.magnitude;
      

        if (distance > Range.Value) return true;

        Found.Value = true; 
        return false;
    }
}