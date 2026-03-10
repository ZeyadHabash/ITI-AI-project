using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "Is Target Outside Range", story: "Is [Target] farther than [Range] from [Self]", category: "Conditions", id: "c9df9f36298f4df7a5d726c14f18e2fa")]
public partial class IsTargetOutsideRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Range;

    public override bool IsTrue()
    {
        if (Self?.Value == null || Target?.Value == null)
        {
            return false;
        }

        float maxRange = Mathf.Max(0.01f, Range != null ? Range.Value : 0f);
        return Vector3.Distance(Self.Value.transform.position, Target.Value.transform.position) > maxRange;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
