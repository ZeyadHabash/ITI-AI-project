using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "Is Target Within Range", story: "Is [Target] within [Range] of [Self]", category: "Conditions", id: "7b1f225cfaa84d16a638ffecf9678b76")]
public partial class IsTargetWithinRangeCondition : Condition
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
        return Vector3.Distance(Self.Value.transform.position, Target.Value.transform.position) <= maxRange;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
