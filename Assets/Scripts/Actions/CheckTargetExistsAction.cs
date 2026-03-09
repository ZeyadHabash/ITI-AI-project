using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckTarget", story: "Check if [Target] is null depending on setting", category: "Action", id: "f36178fb120b8e83516bc092e8a7c391")]
public partial class CheckTargetExistsAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<bool> checkForNull;

    protected override Status OnUpdate()
    {
        bool isNull = Target.Value == null;

        if (checkForNull)
            return isNull ? Status.Success : Status.Failure;
        else
            return !isNull ? Status.Success : Status.Failure;
    }
}