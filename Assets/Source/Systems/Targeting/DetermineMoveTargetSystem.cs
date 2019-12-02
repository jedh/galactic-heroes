using GH.Components;
using GH.SystemGroups;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleLogicSystemGroup))]
	public class DetermineMoveTargetSystem : JobComponentSystem
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
            var translationData = GetComponentDataFromEntity<Translation>(true);

            BeginDeploymentJob beginDeploymentJob = new BeginDeploymentJob(){ TranslationData = translationData, CommandBuffer = commandBuffer };
            UpdateDeploymentJob updateDeploymentJob = new UpdateDeploymentJob(){ TranslationData = translationData };

            var updateJobHandle = updateDeploymentJob.Schedule(this, inputDeps);
            var beginJobHandle = beginDeploymentJob.Schedule(this, updateJobHandle);

            m_EntityCommandBufferSystem.AddJobHandleForProducer(beginJobHandle);

            return beginJobHandle;
        }

        //--------------------------
        // Jobs
        //--------------------------

        [ExcludeComponent(typeof(DeployToPosition))]
        struct BeginDeploymentJob : IJobForEachWithEntity<Target, Translation>
        {
            [ReadOnly]
            public ComponentDataFromEntity<Translation> TranslationData;
            public EntityCommandBuffer.Concurrent CommandBuffer;

            public void Execute(Entity entity, int jobIndex, [ReadOnly] ref Target target, [ReadOnly] ref Translation translation)
            {
                if (TranslationData.Exists(target.TargetEntity))
                {
                    var position = TranslationData[target.TargetEntity];
                    CommandBuffer.AddComponent(jobIndex, entity, new DeployToPosition() { Position = position.Value });
                }
            }
        }

        struct UpdateDeploymentJob : IJobForEachWithEntity<Target, DeployToPosition, Translation, MovementStats>
        {
            [ReadOnly]
            public ComponentDataFromEntity<Translation> TranslationData;

            public void Execute(Entity entity, int jobIndex, [ReadOnly] ref Target target, ref DeployToPosition deployToPosition, [ReadOnly] ref Translation translation, [ReadOnly] ref MovementStats movementStats)
            {
				if (TranslationData.Exists(target.TargetEntity))
				{
                    deployToPosition.Position = TranslationData[target.TargetEntity].Value;
                    deployToPosition.ShouldStop = !movementStats.DoesSwarm;
				}
            }
        }		
	}
}
