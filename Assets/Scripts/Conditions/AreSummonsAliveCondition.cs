using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "Are Summons Alive", story: "Are entries in [Summons] alive", category: "Conditions", id: "c46a8fe2b7ed4c10ad7990475f3c2b11")]
public partial class AreSummonsAliveCondition : Condition
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Summons;
    [SerializeReference] public BlackboardVariable<bool> HasAliveSummons;

    public override bool IsTrue()
    {
        if (Summons.Value == null)
        {
            Summons.Value = new List<GameObject>();
        }

        for (int i = Summons.Value.Count - 1; i >= 0; i--)
        {
            if (Summons.Value[i] == null)
            {
                Summons.Value.RemoveAt(i);
            }
        }

        bool hasAlive = Summons.Value.Count > 0;
        if (HasAliveSummons != null)
        {
            HasAliveSummons.Value = hasAlive;
        }

        return hasAlive;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
