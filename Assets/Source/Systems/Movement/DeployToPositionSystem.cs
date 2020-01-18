using GH.Utils;
using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleLogicSystemGroup)), UpdateBefore(typeof(TranslationSystem)), UpdateBefore(typeof(RotationSystem))]  // updates before so that deltaTime calculations are correct.
	[UpdateAfter(typeof(FindTargetSystem))]
	//[UpdateAfter(typeof(DetermineMoveTargetSystem))]
	[UpdateAfter(typeof(PursueTargetSystem))]
    public class DeployToPositionSystem : JobComponentSystem
	{
		private const float k_DistanceTolerance = 0.02f;
		private const float k_AngleTolerance = 0.9999f;

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
			NativeQueue<CompletedDeployment> queue = new NativeQueue<CompletedDeployment>(Allocator.TempJob);

			var deployJobHandle = new DeployToPositionJob()
			{
				DeltaTime = Time.deltaTime,
				CompletedDeployments = queue.AsParallelWriter()
			}.Schedule(this, inputDeps);

			var removeJobHandle = new RemoveCompletedDeploymentsJob()
			{
				CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
				ToRemoveQueue = queue
			}.Schedule(deployJobHandle); ;

			var queueCleanupJobHandle = queue.Dispose(removeJobHandle);

			m_EntityCommandBufferSystem.AddJobHandleForProducer(queueCleanupJobHandle);

			return queueCleanupJobHandle;
		}

		//--------------------------
		// Jobs
		//--------------------------

		private struct CompletedDeployment
		{
			public int JobIndex;
			public Entity Entity;
		}

		[BurstCompile]
		private struct DeployToPositionJob : IJobForEachWithEntity<DeployToPosition, MovementStats, Rotation, Translation, Velocity, AngularVelocity>
		{
			[WriteOnly]
			public NativeQueue<CompletedDeployment>.ParallelWriter CompletedDeployments;

			[ReadOnly]
			public float DeltaTime;

			public void Execute(Entity entity,
								int jobIndex,
								[ReadOnly] ref DeployToPosition deployment,
								[ReadOnly] ref MovementStats stats,
								[ReadOnly] ref Rotation rotation,
								ref Translation translation,
								ref Velocity velocity,
								ref AngularVelocity angularVelocity)
			{
				NaNDebugger.IsNan(deployment.Position.x, $"[{jobIndex}, {entity.Index}] deployment.Position");
				NaNDebugger.IsNan(translation.Value.x, $"[{jobIndex}, {entity.Index}] translation");

				float3 toTarget = deployment.Position - translation.Value;
				float distance = math.length(toTarget);

				if (deployment.ShouldStop && distance <= k_DistanceTolerance)
				{
					velocity.Value = float3.zero;
					angularVelocity.Velocity = 0f;
					translation.Value = deployment.Position;

					//CompletedDeployments.Enqueue(new CompletedDeployment() { Entity = entity, JobIndex = jobIndex });

					return; // we're there, stop.
				}

				float speed = math.length(velocity.Value);

				float3 forward = math.normalize(math.forward(rotation.Value));
				float3 velocityDirection = speed == 0f ? forward : math.normalize(velocity.Value);

				toTarget = toTarget / distance; // normalize
				float costheta = distance == 0f ? 1 : math.dot(toTarget, forward);



				// Rotation
				if (distance != 0f)
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
						rotationSpeed = (float.IsNaN(angleRadians) ? 0f : angleRadians) * rotationDirection / DeltaTime;    // take us directly there this frame.
					}

					velocityDirection = math.rotate(quaternion.AxisAngle(rotationAxis, rotationSpeed * DeltaTime), velocityDirection);

					angularVelocity.Axis = rotationAxis;
					angularVelocity.Velocity = rotationSpeed;
				}

				NaNDebugger.IsNan(angularVelocity.Axis.x, $"[{jobIndex}, {entity.Index}] rotationAxis");
				NaNDebugger.IsNan(angularVelocity.Axis.x, $"[{jobIndex}, {entity.Index}] rotationSpeed");
				NaNDebugger.IsNan(velocityDirection.x, $"[{jobIndex}, {entity.Index}] velocityDirection");

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
								speed = math.clamp(speed - 0.5f * stats.Deceleration * DeltaTime, 0f, stats.TopSpeed);   // t(v - at/2)
							}
							else
							{
								speed = math.clamp(speed + 0.5f * stats.Acceleration * DeltaTime, 0f, stats.TopSpeed);   // t(v - at/2)
							}

							speedInDirection = math.dot(toTarget, velocityDirection * speed);

							float distanceThisFrame = distance - speedInDirection * DeltaTime;
							if (distanceThisFrame < k_DistanceTolerance)
							{
								speed = distance / DeltaTime; // take us right there this frame.
							}
						}
						else
						{
							// don't stop, just keep accelerating.
							speed = math.clamp(speed + 0.5f * stats.Acceleration * DeltaTime, 0f, stats.TopSpeed);   // t(v - at/2)
						}
					}
					else
					{
						// decelerate
						speed = math.clamp(speed - 0.5f * stats.Deceleration * DeltaTime, 0f, stats.TopSpeed);   // t(v - at/2)
					}

					velocity.Value = velocityDirection * speed;
				}

				NaNDebugger.IsNan(velocity.Value.x, $"[{jobIndex}, {entity.Index}] velocity");
			}
		}

		private struct RemoveCompletedDeploymentsJob : IJob
		{
			public NativeQueue<CompletedDeployment> ToRemoveQueue;

			[WriteOnly]
			public EntityCommandBuffer.Concurrent CommandBuffer;

			public void Execute()
			{
				while (ToRemoveQueue.TryDequeue(out CompletedDeployment r))
				{
					CommandBuffer.RemoveComponent<DeployToPosition>(r.JobIndex, r.Entity);
				}
			}
		}
	}
}