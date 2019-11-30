using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup)), UpdateBefore(typeof(TranslationSystem)), UpdateBefore(typeof(RotationSystem))]  // updates before so that deltaTime calculations are correct.
    public class DeployToPositionSystem : ComponentSystem
    {
        private const float k_DistanceTolerance = 0.02f;
        private const float k_AngleTolerance = 0.9999f;

        protected override void OnUpdate()
        {
            Entities.ForEach(
                (Entity entity,
                ref DeployToPosition deployment, ref MovementStats stats, ref Translation translation, ref Rotation rotation, ref Velocity velocity, ref AngularVelocity angularVelocity) =>
            {
                float3 toTarget = deployment.Position - translation.Value;
                float distance = math.length(toTarget);

                if (deployment.ShouldStop && distance <= k_DistanceTolerance)
                {
                    velocity.Value = float3.zero;
                    angularVelocity.Velocity = 0f;
                    translation.Value = deployment.Position;

                    PostUpdateCommands.RemoveComponent<DeployToPosition>(entity);

                    return; // we're there, stop.
                }


                float speed = math.length(velocity.Value);

                float3 forward = math.normalize(math.forward(rotation.Value));
                float3 velocityDirection = speed == 0f ? forward : math.normalize(velocity.Value);

                toTarget = toTarget / distance; // normalize
                float costheta = math.dot(toTarget, forward);



                // Rotation
                {
                    float3 rotationAxis = costheta == -1 || costheta == 1 ? math.up() : math.normalize(math.cross(forward, toTarget));
                    float rotationDirection = math.saturate(math.sign(math.dot(rotationAxis, math.up())) + 1) * 2 - 1;  // -1 or 1
                    float rotationSpeed = math.radians(stats.RotationSpeed) * rotationDirection;
                    rotationAxis = rotationAxis * rotationDirection;

                    if (costheta >= stats.ThrustTolerance)  // have we turned enough to begin moving forward?
                    {
                        velocityDirection = forward;
                    }
                    else
                    {
                        rotationSpeed *= math.step(speed, /* < */ stats.MaxSpeedToTurn);    // only turn if we've slowed down enough
                    }

                    if (costheta >= k_AngleTolerance)
                    {
                        float angleRadians = math.acos(costheta);
                        rotationSpeed = (float.IsNaN(angleRadians) ? 0f : angleRadians) * rotationDirection / Time.deltaTime;    // take us directly there this frame.
                    }

                    velocityDirection = math.rotate(quaternion.AxisAngle(rotationAxis, rotationSpeed * Time.deltaTime), velocityDirection);

                    angularVelocity.Axis = rotationAxis;
                    angularVelocity.Velocity = rotationSpeed;
                }



                // Translation
                {
                    if (costheta >= stats.ThrustTolerance)  // have we turned enough to begin moving forward?
                    {
                        if (deployment.ShouldStop)
                        {
                            // https://math.stackexchange.com/questions/233107/finding-minimum-distance-traveled-with-specified-deceleration-from-starting-spee
                            float speedInDirection = math.dot(toTarget, velocityDirection * speed);
                            float distanceTravelledIfDecelerating = (speedInDirection * speedInDirection) / (2 * stats.Deceleration);

                            if (distanceTravelledIfDecelerating >= distance)
                            {
                                speed = math.clamp(speed - 0.5f * stats.Deceleration * Time.deltaTime, 0f, stats.TopSpeed);   // t(v - at/2)
                            }
                            else
                            {
                                speed = math.clamp(speed + 0.5f * stats.Acceleration * Time.deltaTime, 0f, stats.TopSpeed);   // t(v - at/2)
                            }

                            speedInDirection = math.dot(toTarget, velocityDirection * speed);

                            float distanceThisFrame = distance - speedInDirection * Time.deltaTime;
                            if (distanceThisFrame < k_DistanceTolerance)
                            {
                                speed = distance / Time.deltaTime; // take us right there this frame.
                            }
                        }
                        else
                        {
                            // don't stop, just keep accelerating.
                            speed = math.clamp(speed + 0.5f * stats.Acceleration * Time.deltaTime, 0f, stats.TopSpeed);   // t(v - at/2)
                        }
                    }
                    else
                    {
                        // decelerate
                        speed = math.clamp(speed - 0.5f * stats.Deceleration * Time.deltaTime, 0f, stats.TopSpeed);   // t(v - at/2)
                    }

                    velocity.Value = velocityDirection * speed;
                }
            });
        }
    }
}