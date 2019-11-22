using GH.Components;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Systems
{
    public class TranslateToSystem : ComponentSystem
    {
        private const float k_DistanceFromTarget = 0.001f;

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref TranslateToPosition target, ref MovementStats stats, ref Velocity velocity, ref Translation translation) =>
            {
                float3 toTarget = target.Value - translation.Value;

                float distance = math.length(toTarget);
                if(distance <= k_DistanceFromTarget)
                {
                    EntityManager.RemoveComponent<TranslateToPosition>(entity);
                    EntityManager.RemoveComponent<Velocity>(entity);
                    translation.Value = target.Value;
                    return; // we're there, stop.
                }

                float speed = math.length(velocity.Value);  // starts with existing speed.

                float v = Mathf.Min(speed + stats.Acceleration * Time.deltaTime * Time.deltaTime, stats.TopSpeed);

                if ((distance - v * Time.deltaTime) < 0)    // expected movement
                {
                    v = distance / Time.deltaTime;
                }

                velocity.Value = math.normalize(toTarget) * v;
            });
        }
    }
}