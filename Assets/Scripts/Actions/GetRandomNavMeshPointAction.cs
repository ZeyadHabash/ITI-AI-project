using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get Random NavMesh Point", story: "Get random point around [self] within radius [radius] and store result in [Result]", category: "Action", id: "1ab2ecc3c4c01ef47684767e0ccc4c69")]
public partial class GetRandomNavMeshPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<Vector3> Result;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
          
            return Status.Failure;
        }

  
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * Radius.Value;
        randomDirection += Self.Value.transform.position;


        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, Radius.Value, NavMesh.AllAreas))
        {
            Result.Value = hit.position;
            return Status.Success;
        }

        return Status.Failure;
    }


    protected override void OnEnd()
    {
    }
}

