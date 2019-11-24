using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
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
                    velocity.Value = float3.zero;
                    translation.Value = target.Value;

                    PostUpdateCommands.RemoveComponent<TranslateToPosition>(entity);

                    return; // we're there, stop.
                }

                float speed = math.length(velocity.Value);  // starts with existing speed.

                // https://math.stackexchange.com/questions/233107/finding-minimum-distance-traveled-with-specified-deceleration-from-starting-spee
                float distanceTravelledIfDecelerating = (speed * speed) / (2 * stats.Deceleration);

                float v = speed;
                if (distance <= distanceTravelledIfDecelerating)
                {
                    v = math.min(v - stats.Deceleration * Time.deltaTime * Time.deltaTime, stats.TopSpeed);
                }
                else
                {
                    // https://math.stackexchange.com/questions/637042/calculate-maximum-velocity-given-accel-decel-initial-v-final-position
                    float a = stats.Acceleration;
                    float d = stats.Deceleration;
                    float vmax = math.min(math.sqrt((d * speed * speed + 2 * a * distance * d) / (a + d)), stats.TopSpeed);

                    v = math.min(speed + stats.Acceleration * Time.deltaTime * Time.deltaTime, vmax);
                }

                float expectedDistanceThisFrame = distance - v * Time.deltaTime;

                if (expectedDistanceThisFrame < k_DistanceFromTarget)
                {
                    v = distance / Time.deltaTime; // take us right there this frame.
                }

                velocity.Value = math.normalize(toTarget) * v;
            });
        }
    }
}