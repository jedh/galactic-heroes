using GH.Components;
using GH.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class BasicWeaponsSystem : JobComponentSystem
    {
        private EntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            m_EntityCommandBufferSystem = null;
            base.OnDestroy();
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            FireWeaponJob fireWeaponJob = new FireWeaponJob() { CommandBuffer = commandBuffer };

            var beginJobHandle = fireWeaponJob.Schedule(this, inputDeps);

            m_EntityCommandBufferSystem.AddJobHandleForProducer(beginJobHandle);

            return beginJobHandle;
        }

        //--------------------------
        // Jobs
        //--------------------------

        [ExcludeComponent(typeof(DeployToPosition))]
        struct FireWeaponJob : IJobForEachWithEntity<Target>
        {
            public EntityCommandBuffer.Concurrent CommandBuffer;

            public void Execute(Entity entity, int jobIndex, [ReadOnly] ref Target target)
            {
                // determine if we should fire
                    // Check distance to target
                    // firing arc
                    // friendly fire?

                // create flag to fire weapons
            }
        }
    }  
 }
