using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class DeployToPositionSystem : ComponentSystem
    {
        private const float k_DistanceFromTarget = 0.001f;

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref MovementStats stats, ref Translation translation, ref Velocity velocity, ref Rotation rotation, ref DeployToPosition moveToPosition) =>
            {
                PostUpdateCommands.RemoveComponent<DeployToPosition>(entity);
                PostUpdateCommands.RemoveComponent<MoveForward>(entity);

                PostUpdateCommands.AddComponent<Moving>(entity);
                PostUpdateCommands.SetComponent(entity, new Moving() { Value = moveToPosition.Value });

                PostUpdateCommands.AddComponent<RotateTowardsPosition>(entity);
                PostUpdateCommands.SetComponent(entity, new RotateTowardsPosition() { Value = moveToPosition.Value });
            });

            Entities.WithNone<MoveForward>().ForEach((Entity entity, ref MovementStats stats, ref Translation translation, ref Velocity velocity, ref Rotation rotation, ref Moving moving) =>
            {
                float3 forward = math.normalize(math.forward(rotation.Value));

                float cosAngle = math.dot(forward, math.normalize(moving.Value - translation.Value));

                if (math.abs(cosAngle) <= stats.ThrustTolerance) // have we turned enough to begin moving forward?
                {
                    float currentForwardSpeed = math.dot(forward, velocity.Value);
                    float forwardSpeed = math.min(currentForwardSpeed + stats.Acceleration * Time.deltaTime * Time.deltaTime, stats.TopSpeed);

                    PostUpdateCommands.AddComponent<MoveForward>(entity);
                    PostUpdateCommands.SetComponent(entity, new MoveForward() { Speed = forwardSpeed });
                }
            });

            Entities.ForEach((Entity entity, ref MovementStats stats, ref Translation translation, ref Velocity velocity, ref Rotation rotation, ref Moving moving, ref MoveForward moveForward) =>
            {
                moveForward.Speed = math.min(moveForward.Speed + stats.Acceleration * Time.deltaTime * Time.deltaTime, stats.TopSpeed);

                float3 toTarget = moving.Value - translation.Value;

                float distance = math.length(toTarget);
                if (distance <= k_DistanceFromTarget)
                {
                    velocity.Value = float3.zero;
                    translation.Value = moving.Value;

                    PostUpdateCommands.RemoveComponent<Moving>(entity);
                    PostUpdateCommands.RemoveComponent<MoveForward>(entity);

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