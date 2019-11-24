using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class RotateTowardsPositionSystem : ComponentSystem
    {
        private const float k_RadiansTolerance = 0.01745329f;

        protected override void OnUpdate()
        {
            Entities.ForEach(
                (Entity entity, ref RotateTowardsPosition target, 
                ref MovementStats stats, ref Rotation rotation, ref AngularVelocity angularVelocity, ref Translation translation) =>
            {
                float3 toPos = math.normalize(target.Value - translation.Value);
                float3 forward = math.normalize(math.forward(rotation.Value));

                float costheta = math.dot(toPos, forward);

                float3 axis = costheta == -1 || costheta == 1 ? math.up() : math.cross(forward, toPos);
                float angleRadians = math.acos(costheta);

                float dot = math.dot(axis, math.up());
                float direction = dot == 0f ? 1f : dot / math.abs(dot); // [-1, 1]

                axis = math.normalize(axis) * direction;
                
                if(angleRadians <= k_RadiansTolerance)
                {
                    angularVelocity.Axis = axis;
                    angularVelocity.Velocity = 0f;

                    rotation.Value = math.mul(rotation.Value, quaternion.AxisAngle(axis, angleRadians));

                    PostUpdateCommands.RemoveComponent<RotateTowardsPosition>(entity);

                    return;
                }

                angularVelocity.Axis = axis;
                angularVelocity.Velocity = math.radians(stats.RotationSpeed) * direction;

                float expectedV = angularVelocity.Velocity * Time.deltaTime;
                if (math.abs(angleRadians) - math.abs(expectedV) <= 0f )
                {
                    angularVelocity.Velocity = angleRadians / Time.deltaTime;
                }
            });
        }
    }
}