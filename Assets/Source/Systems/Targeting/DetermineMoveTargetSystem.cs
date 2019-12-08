using GH.Utils;
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

			BeginDeploymentJob beginDeploymentJob = new BeginDeploymentJob() { TranslationData = translationData, CommandBuffer = commandBuffer };
			UpdateDeploymentJob updateDeploymentJob = new UpdateDeploymentJob() { TranslationData = translationData };
			DeploymentFinishedJob deploymentFinishedJob = new DeploymentFinishedJob() { CommandBuffer = commandBuffer };

			var updateJobHandle = updateDeploymentJob.Schedule(this, inputDeps);
			var beginJobHandle = beginDeploymentJob.Schedule(this, updateJobHandle);
			var finishedJobHandle = deploymentFinishedJob.Schedule(this, beginJobHandle);

			m_EntityCommandBufferSystem.AddJobHandleForProducer(finishedJobHandle);

			return finishedJobHandle;
		}

		//--------------------------
		// Jobs
		//--------------------------

		[ExcludeComponent(typeof(DeployToPosition), typeof(CombatMovement))]
		struct BeginDeploymentJob : IJobForEachWithEntity<Target, Translation, WeaponStats, MovementStats>
		{
			[ReadOnly]
			public ComponentDataFromEntity<Translation> TranslationData;
			public EntityCommandBuffer.Concurrent CommandBuffer;

			public void Execute(Entity entity, int jobIndex, [ReadOnly] ref Target target, [ReadOnly] ref Translation translation, [ReadOnly] ref WeaponStats weaponStats, [ReadOnly] ref MovementStats movementStats)
			{
				if (TranslationData.Exists(target.TargetEntity))
				{
					var position = TranslationData[target.TargetEntity];
					CommandBuffer.AddComponent(jobIndex, entity, new DeployToPosition() { Position = position.Value, ShouldStop = !movementStats.DoesSwarm });
					CommandBuffer.AddComponent(jobIndex, entity, new CombatMovement()
					{
						MinRangeSq = weaponStats.MinRange * weaponStats.MinRange,
						MaxRangeSq = weaponStats.MaxRange * weaponStats.MaxRange,
						OptimalRangeSq = weaponStats.OptimalRange * weaponStats.OptimalRange
					});
				}
			}
		}

		[BurstCompile]
		struct UpdateDeploymentJob : IJobForEachWithEntity<Target, DeployToPosition, Translation, CombatMovement>
		{
			[ReadOnly]
			public ComponentDataFromEntity<Translation> TranslationData;

			public void Execute(Entity entity, int jobIndex, [ReadOnly] ref Target target, ref DeployToPosition deployToPosition, [ReadOnly] ref Translation translation, [ReadOnly] ref CombatMovement combatMovement)
			{
				if (TranslationData.Exists(target.TargetEntity))
				{
					var targetPosition = TranslationData[target.TargetEntity];
					var distanceSq = math.distancesq(targetPosition.Value, translation.Value);
					if (distanceSq > combatMovement.OptimalRangeSq)
					{
						if (combatMovement.OptimalRangeSq != 0f)
						{
							var direction = targetPosition.Value - translation.Value;
							direction = math.normalizesafe(direction);
							var optimalPosition = translation.Value + (direction * math.sqrt(combatMovement.OptimalRangeSq));

                            NaNDebugger.IsNan(targetPosition.Value.x, "target nan");

							deployToPosition.Position = optimalPosition;
						}
						else
						{
							deployToPosition.Position = targetPosition.Value;
						}
					}
					else
					{
						deployToPosition.Position = translation.Value;
					}
				}
			}
		}

		[ExcludeComponent(typeof(DeployToPosition))]
		struct DeploymentFinishedJob : IJobForEachWithEntity<CombatMovement>
		{
			public EntityCommandBuffer.Concurrent CommandBuffer;

			public void Execute(Entity entity, int jobIndex, [ReadOnly] ref CombatMovement combatMovement)
			{
				CommandBuffer.RemoveComponent<CombatMovement>(jobIndex, entity);
			}
		}
	}
}
