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
        private const float k_Tolerance = 0.9999f;

        protected override void OnUpdate()
        {
            Entities.ForEach(
                (Entity entity, ref RotateTowardsPosition target, 
                ref MovementStats stats, ref Rotation rotation, ref AngularVelocity angularVelocity, ref Translation translation) =>
            {
                float3 toPos = target.Value - translation.Value;

                if(math.lengthsq(toPos) == 0f)
                {
                    angularVelocity.Velocity = 0f;
                    PostUpdateCommands.RemoveComponent<RotateTowardsPosition>(entity);
                    return;
                }

                toPos = math.normalize(toPos);

                float3 forward = math.normalize(math.forward(rotation.Value));

                float costheta = math.dot(toPos, forward);

                float3 axis = costheta == -1 || costheta == 1 ? math.up() : math.cross(forward, toPos);
                float angleRadians = math.acos(costheta);

                float dot = math.dot(axis, math.up());
                float direction = dot == 0f ? 1f : dot / math.abs(dot); // [-1, 1]

                axis = math.normalize(axis) * direction;
                
                if(costheta >= k_Tolerance)
                {
                    angularVelocity.Axis = axis;
                    angularVelocity.Velocity = 0f;

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