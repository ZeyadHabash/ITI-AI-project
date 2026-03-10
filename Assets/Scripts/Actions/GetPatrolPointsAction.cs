using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetPatrolPointsAction", story: "Get [points] in [pointList]", category: "Action", id: "f566e3c167f0ba165d9a7f6f2024e925")]
public partial class GetPatrolPointsAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Points;
    [SerializeReference] public BlackboardVariable<GetPatrolPoints> PointList;

    protected override Status OnStart()
    {
        PointList = (BlackboardVariable<GetPatrolPoints>)GameObject.Find("Points").GetComponent<GetPatrolPoints>();
        Debug.Log(PointList.Value);
        if (PointList.Value == null)
        {
            return Status.Failure;
        }
        Points.Value = PointList.Value.Points;
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

