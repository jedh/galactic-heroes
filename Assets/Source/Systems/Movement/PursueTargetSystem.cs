using GH.Components;
using GH.SystemGroups;
using GH.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleLogicSystemGroup))]
	public class PursueTargetSystem : JobComponentSystem
	{
		private EntityCommandBufferSystem m_EntityCommandBufferSystem;

		// Increase/decrease this value for different pursuit effectiveness.
		private float m_LookAheadTimeStep = 3f;

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
			var velocityData = GetComponentDataFromEntity<Velocity>(true);

			BeginPursuitJob beginDeploymentJob = new BeginPursuitJob()
			{
				TranslationData = translationData,
				VelocityData = velocityData,
				LookAheadTimeStep = m_LookAheadTimeStep,
				CommandBuffer = commandBuffer,
				DeltaTime = Time.deltaTime
			};

			UpdatePursuitJob updateDeploymentJob = new UpdatePursuitJob()
			{
				TranslationData = translationData,
				VelocityData = velocityData,
				LookAheadTimeStep = m_LookAheadTimeStep,
				DeltaTime = Time.deltaTime
			};

			PursuitJobFinished deploymentFinishedJob = new PursuitJobFinished() { CommandBuffer = commandBuffer };

			var updateJobHandle = updateDeploymentJob.Schedule(this, inputDeps);
			var beginJobHandle = beginDeploymentJob.Schedule(this, updateJobHandle);
			var finishedJobHandle = deploymentFinishedJob.Schedule(this, beginJobHandle);

			m_EntityCommandBufferSystem.AddJobHandleForProducer(finishedJobHandle);

			return finishedJobHandle;
		}

		//////////
		// JOBS //
		//////////

		// Add pursuit movment if it doesn't exist.
		[ExcludeComponent(typeof(PursueTarget), typeof(CombatMovement))]
		struct BeginPursuitJob : IJobForEachWithEntity<Target, WeaponStats, MovementStats, DeployToPosition>
		{
			[ReadOnly]
			public ComponentDataFromEntity<Translation> TranslationData;

			[ReadOnly]
			public ComponentDataFromEntity<Velocity> VelocityData;

			public EntityCommandBuffer.Concurrent CommandBuffer;

			[ReadOnly]
			public float LookAheadTimeStep;

			[ReadOnly]
			public float DeltaTime;

			public void Execute(Entity entity, int index, [ReadOnly] ref Target target, [ReadOnly] ref WeaponStats weaponStats, [ReadOnly] ref MovementStats movementStats, ref DeployToPosition deployToPosition)
			{
				var targetPosition = TranslationData[target.TargetEntity];
				var targetVelocity = VelocityData[target.TargetEntity];
				var desiredPosition = targetPosition.Value + targetVelocity.Value * LookAheadTimeStep * DeltaTime;
				deployToPosition.Position = desiredPosition;
				deployToPosition.ShouldStop = !movementStats.DoesSwarm;
				//CommandBuffer.SetComponent(jobIndex, entity, new DeployToPosition() { Position = position.Value, ShouldStop = !movementStats.DoesSwarm });
				CommandBuffer.AddComponent(index, entity, new CombatMovement()
				{
					MinRangeSq = weaponStats.MinRange * weaponStats.MinRange,
					MaxRangeSq = weaponStats.MaxRange * weaponStats.MaxRange,
					OptimalRangeSq = weaponStats.OptimalRange * weaponStats.OptimalRange
				});

				CommandBuffer.AddComponent(index, entity, default(PursueTarget));
			}
		}

		// Update pursuit targets based on whether ships swarm or not.       
		struct UpdatePursuitJob : IJobForEachWithEntity<Target, DeployToPosition, Translation, CombatMovement, PursueTarget, MovementStats>
		{
			[ReadOnly]
			public ComponentDataFromEntity<Translation> TranslationData;

			[ReadOnly]
			public ComponentDataFromEntity<Velocity> VelocityData;

			[ReadOnly]
			public float LookAheadTimeStep;

			[ReadOnly]
			public float DeltaTime;

			public void Execute(Entity entity, int index, [ReadOnly] ref Target target, ref DeployToPosition deployToPosition, [ReadOnly] ref Translation translation, [ReadOnly] ref CombatMovement combatMovement, [ReadOnly] ref PursueTarget pursueTarget, [ReadOnly] ref MovementStats movementStats)
			{
				if (TranslationData.Exists(target.TargetEntity) &&
					VelocityData.Exists(target.TargetEntity))
				{
					var targetPosition = TranslationData[target.TargetEntity];
					var targetVelocity = VelocityData[target.TargetEntity];
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
							// This version ignores the timestep and uses relative distance instead.
							var t = math.length(math.sqrt(distanceSq)) * movementStats.TopSpeed;
							var desiredPosition = targetPosition.Value + targetVelocity.Value * t * DeltaTime;

							//var desiredPosition = targetPosition.Value + targetVelocity.Value * LookAheadTimeStep * DeltaTime;
							deployToPosition.Position = desiredPosition;
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
		struct PursuitJobFinished : IJobForEachWithEntity<CombatMovement, PursueTarget>
		{
			public EntityCommandBuffer.Concurrent CommandBuffer;

			public void Execute(Entity entity, int index, [ReadOnly] ref CombatMovement combatMovement, [ReadOnly] ref PursueTarget pursueTarget)
			{
				CommandBuffer.RemoveComponent<CombatMovement>(index, entity);
				CommandBuffer.RemoveComponent<PursueTarget>(index, entity);
			}
		}
	}
}
