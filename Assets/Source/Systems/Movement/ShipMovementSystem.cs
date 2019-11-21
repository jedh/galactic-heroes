using GH.Components;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Systems
{
    public class ShipMovementSystem : ComponentSystem
    {
        private const float k_DistanceFromTarget = 0.001f;

        protected override void OnUpdate()
        {
            Entities.ForEach(
                (Entity entity, 
                ref MovementTarget target, 
                ref MovementStats stats, 
                ref MoveSpeed speed, 
                ref Translation translation, 
                ref Rotation rotation) =>
            {
                float3 toTarget = target.Value - translation.Value;
                float distance = math.length(toTarget);

                if(distance <= k_DistanceFromTarget)
                {
                    EntityManager.RemoveComponent<MovementTarget>(entity);
                    EntityManager.RemoveComponent<MoveSpeed>(entity);
                    translation.Value = target.Value;
                    return; // we're there, stop.
                }

                float v = Mathf.Min(speed.Value + stats.Acceleration * Time.deltaTime * Time.deltaTime, stats.TopSpeed);

                if ((distance - v * Time.deltaTime) < 0)    // expected movement
                {
                    v = distance / Time.deltaTime;
                }

                speed.Value = v;
                rotation.Value = quaternion.LookRotation(math.normalize(toTarget), math.up());
            });
        }
    }
}