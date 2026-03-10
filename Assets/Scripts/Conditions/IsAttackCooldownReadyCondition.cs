using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "Is Attack Cooldown Ready", story: "Is attack cooldown ready using [LastAttackTime] and [AttackCooldown]", category: "Conditions", id: "9c4c1c9e91464258b62a9a6f7eef359f")]
public partial class IsAttackCooldownReadyCondition : IsCooldownReadyCondition
{
    [SerializeReference] public BlackboardVariable<float> LastAttackTime;
    [SerializeReference] public BlackboardVariable<float> AttackCooldown;
    [SerializeReference] public BlackboardVariable<float> RemainingAttackCooldown;

    public override bool IsTrue()
    {
        return EvaluateCooldown(LastAttackTime, AttackCooldown, RemainingAttackCooldown);
    }
}