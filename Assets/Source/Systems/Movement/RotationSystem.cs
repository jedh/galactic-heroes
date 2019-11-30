using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using GH.Components;
using GH.SystemGroups;
using UnityEngine;
using Unity.Jobs;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class RotationSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new RotationJob() { DeltaTime = Time.deltaTime }.Schedule(this, inputDeps);
        }

        struct RotationJob : IJobForEach<Rotation, AngularVelocity>
        {
            public float DeltaTime;

            [BurstCompile]
            public void Execute(ref Rotation rotation, ref AngularVelocity angularVelocity)
            {
                if (angularVelocity.Velocity != 0f && (angularVelocity.Axis.x != 0f || angularVelocity.Axis.y != 0f || angularVelocity.Axis.z != 0f))
                {
                    rotation.Value = math.mul(rotation.Value, quaternion.AxisAngle(angularVelocity.Axis, angularVelocity.Velocity * DeltaTime));
                }
            }
        }
    }
}