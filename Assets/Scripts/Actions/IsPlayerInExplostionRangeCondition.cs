using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsPlayerInExplostionRange", story: "is [target] in explosion range [Range] from [Self]", category: "Conditions", id: "060e13e8bd6de733daa27009128fba5e")]
public partial class IsPlayerInExplostionRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        Vector3 directionToTarget = Target.Value.transform.position - Self.Value.transform.position;
        float distance = directionToTarget.magnitude;
        Debug.Log(distance);

        if (distance > Range.Value)
        {
            Debug.Log("FALSE");
            return true;
        }
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
