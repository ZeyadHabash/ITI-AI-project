using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayParticle", story: "PlayParticle [particle]", category: "Action", id: "cbbf1212457d34412c3e92449fd29f63")]
public partial class PlayParticle : Action
{
    [SerializeReference] public BlackboardVariable<ParticleSystem> Particle;

    protected override Status OnStart()
    {
        if (Particle.Value != null)
        {
            Particle.Value.Play();
            return Status.Success;
        }
        Debug.Log("particle failed");
        return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return base.OnUpdate();
    }
}

