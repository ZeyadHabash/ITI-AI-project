using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
// I updated the "story" so it reads nicely in your Behavior Graph editor!
[NodeDescription(name: "Explode", story: "[Agent] explodes with [Particle] dealing [Damage] damage in [Radius] radius with [Force] force", category: "Action", id: "cbbf1212457d34412c3e92449fd29f63")]
public partial class Explode : Action
{
  
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<ParticleSystem> Particle;

    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<float> Damage;
    [SerializeReference] public BlackboardVariable<float> Force;

    protected override Status OnStart()
    {
        
        if (Agent.Value == null)
        {
            
            return Status.Failure;
        }

    
        if (Particle.Value != null)
        {
            Particle.Value.Play();
        }


        Vector3 explosionCenter = Agent.Value.transform.position;
        float expRadius = Radius.Value;
        float expDamage = Damage.Value;
        float expForce = Force.Value;


        Collider[] hitColliders = Physics.OverlapSphere(explosionCenter, expRadius);

 
        foreach (Collider hit in hitColliders)
        {
         
            if (hit.gameObject == Agent.Value) continue;

            // Apply Damage
            IDamageable damageableTarget = hit.GetComponent<IDamageable>();
            if (damageableTarget != null)
            {
                damageableTarget.TakeDamage(expDamage);
            }
            //else
            //{
            //    continue;
            //}

          
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                
                Vector3 pushDirection = hit.transform.position - explosionCenter;
                pushDirection.y = Force / 5;
                pushDirection.Normalize();
                
                float distance = Vector3.Distance(explosionCenter, hit.transform.position);
                float forceMultiplier = 1f - (distance / expRadius);

              
                rb.AddForce(pushDirection * (expForce * forceMultiplier), ForceMode.Impulse);

                
            }
        }
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        // We don't need OnUpdate because explosions happen instantly in OnStart.
        return base.OnUpdate();
    }
}