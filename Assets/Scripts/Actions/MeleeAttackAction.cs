using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Melee Attack", story: "[Self] melee attacks [Target] in [Range] for [Damage]", category: "Action", id: "5b98d39ef3e54692a4b6d0ca82ef7412")]
public partial class MeleeAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<int> Damage;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<float> Cooldown;
    [SerializeReference] public BlackboardVariable<float> LastAttackTime;
    [SerializeReference] public BlackboardVariable<Animator> Animator;
    [SerializeReference] public BlackboardVariable<string> AttackTrigger;

    protected override Status OnStart()
    {
        if (Self.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        if (Time.time < LastAttackTime.Value + Mathf.Max(0f, Cooldown.Value))
        {
            return Status.Failure;
        }

        float distance = Vector3.Distance(Self.Value.transform.position, Target.Value.transform.position);
        if (distance > Mathf.Max(0.1f, Range.Value))
        {
            return Status.Failure;
        }

        Animator resolvedAnimator = Animator != null ? Animator.Value : null;
        if (resolvedAnimator == null && Self.Value != null)
        {
            resolvedAnimator = Self.Value.GetComponent<Animator>();
        }

        if (resolvedAnimator != null && !string.IsNullOrWhiteSpace(AttackTrigger.Value))
        {
            resolvedAnimator.SetTrigger(AttackTrigger.Value);
        }

        IDamageable damageable = Target.Value.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(Mathf.Max(1, Damage.Value));
        }

        LastAttackTime.Value = Time.time;
        return Status.Success;
    }
}
