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
        private const float k_DistanceFromTarget = 0.02f;

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref MovementStats stats, ref Translation translation, ref Velocity velocity, ref Rotation rotation, ref DeployToPosition moveToPosition) =>
            {
                PostUpdateCommands.RemoveComponent<RotateTowardsPosition>(entity);
                PostUpdateCommands.RemoveComponent<DeployToPosition>(entity);
                PostUpdateCommands.RemoveComponent<MoveForward>(entity);

                PostUpdateCommands.AddComponent<MovingTo>(entity);
                PostUpdateCommands.SetComponent(entity, new MovingTo() { Position = moveToPosition.Position, ShouldSTop = moveToPosition.ShouldDecelerate });

                PostUpdateCommands.AddComponent<FaceTarget>(entity);
                PostUpdateCommands.SetComponent(entity, new FaceTarget() { Value = moveToPosition.Position });
            });

            Entities.WithNone<MoveForward>().ForEach((Entity entity, ref MovementStats stats, ref Translation translation, ref Velocity velocity, ref Rotation rotation, ref MovingTo target) =>
            {
                float3 toPos = math.normalize(target.Position - translation.Value);
                float3 forward = math.normalize(math.forward(rotation.Value));

                float costheta = math.dot(toPos, forward);

                if (costheta >= stats.ThrustTolerance) // have we turned enough to begin moving forward?
                {
                    float currentForwardSpeed = math.dot(forward, velocity.Value);
                    float forwardSpeed = math.min(currentForwardSpeed + stats.Acceleration * Time.deltaTime * Time.deltaTime, stats.TopSpeed);

                    PostUpdateCommands.AddComponent<MoveForward>(entity);
                    PostUpdateCommands.SetComponent(entity, new MoveForward() { Speed = forwardSpeed });
                }
                else
                {
                    float speed = math.length(velocity.Value);

                    if (speed != 0f)
                    {
                        speed = math.max(speed - stats.Deceleration * Time.deltaTime * 0.5f, 0f);

                        velocity.Value = math.normalize(velocity.Value) * speed;
                    }
                }
            });

            Entities.ForEach((Entity entity, ref MovementStats stats, ref Translation translation, ref Velocity velocity, ref Rotation rotation, ref MovingTo moving, ref MoveForward moveForward) =>
            {
                float3 toTarget = moving.Position - translation.Value;

                float t = Time.deltaTime;
                float a = stats.Acceleration;
                float d = stats.Deceleration;
                float speed = moveForward.Speed;
                float distance = math.length(toTarget);
                float3 forward = math.normalize(math.forward(rotation.Value));

                if (moving.ShouldSTop)
                {
                    if (distance <= k_DistanceFromTarget)
                    {
                        moveForward.Speed = 0f;

                        velocity.Value = float3.zero;
                        translation.Value = moving.Position;

                        PostUpdateCommands.RemoveComponent<MovingTo>(entity);
                        PostUpdateCommands.RemoveComponent<FaceTarget>(entity);
                        PostUpdateCommands.RemoveComponent<MoveForward>(entity);

                        return; // we're there, stop.
                    }

                    float speedInDirection = math.dot(math.normalize(toTarget), forward * speed);

                    // https://math.stackexchange.com/questions/233107/finding-minimum-distance-traveled-with-specified-deceleration-from-starting-spee
                    float distanceTravelledIfDecelerating = (speedInDirection * speedInDirection) / (2 * d);

                    if (distance <= distanceTravelledIfDecelerating)
                    {
                        speed = math.min(speed - d * t * 0.5f, stats.TopSpeed);
                    }
                    else
                    {
                        speed = math.min(speed + a * t * 0.5f, stats.TopSpeed);
                    }

                    speedInDirection = math.dot(math.normalize(toTarget), forward * speed);

                    float expectedDistanceThisFrame = distance - speedInDirection * t;
                    if (expectedDistanceThisFrame < k_DistanceFromTarget)
                    {
                        Debug.LogWarning("Stopping");
                        speed = distance / Time.deltaTime; // take us right there this frame.
                    }
                }
                else
                {
                    speed = math.min(speed + a * t * 0.5f, stats.TopSpeed);
                }

                moveForward.Speed = speed;
            });
        }
    }
}