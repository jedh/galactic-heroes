using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class RotationSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new RotationJob() { DeltaTime = Time.deltaTime }.Schedule(this, inputDeps);
        }

        //--------------------------
        // Jobs
        //--------------------------

        [BurstCompile]
        struct RotationJob : IJobForEach<Rotation, AngularVelocity>
        {
            public float DeltaTime;

            public void Execute(ref Rotation rotation, [ReadOnly] ref AngularVelocity angularVelocity)
            {
                if (angularVelocity.Velocity != 0f && (angularVelocity.Axis.x != 0f || angularVelocity.Axis.y != 0f || angularVelocity.Axis.z != 0f))
                {
                    rotation.Value = math.mul(rotation.Value, quaternion.AxisAngle(angularVelocity.Axis, angularVelocity.Velocity * DeltaTime));
                }
            }
        }
    }
}