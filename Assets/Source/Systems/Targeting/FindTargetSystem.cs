using GH.SystemGroups;
using GH.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleLogicSystemGroup))]
	[UpdateAfter(typeof(RandomizedInitialDeploymentSystem))]
	public class FindTargetSystem : JobComponentSystem
	{
		private EntityQuery m_TargetShipsQuery;

		private EntityCommandBufferSystem m_EntityCommandBufferSystem;

		protected override void OnCreate()
		{
			var targetShipsQueryDesc = new EntityQueryDesc()
			{
				All = new ComponentType[] { ComponentType.ReadOnly<Ship>(), ComponentType.ReadOnly<Translation>() }
			};
			m_TargetShipsQuery = GetEntityQuery(targetShipsQueryDesc);

			m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var targetEntities = m_TargetShipsQuery.ToEntityArray(Allocator.TempJob);
			var targetShips = m_TargetShipsQuery.ToComponentDataArray<Ship>(Allocator.TempJob);
			var targetTranslations = m_TargetShipsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

			var findTargetsJob = new FindTargetsJob()
			{
				TargetEntities = targetEntities,
				TargetShips = targetShips,
				TargetTranslations = targetTranslations
			}.Schedule(this, inputDeps);

			findTargetsJob.Complete();
			targetEntities.Dispose();
			targetShips.Dispose();
			targetTranslations.Dispose();

			var addFoundTargetsJob = new AddTargetsJob()
			{
				CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
			}.Schedule(this, findTargetsJob);

			m_EntityCommandBufferSystem.AddJobHandleForProducer(addFoundTargetsJob);

			return addFoundTargetsJob;
		}

		public struct FindTargetCompleted
		{
			public int JobIndex;
			public Entity TargetingShip;
			public Entity FoundShip;
		}

		[BurstCompile]
		[ExcludeComponent(typeof(Target))]
		public struct FindTargetsJob : IJobForEach<FindTarget, Ship, Translation>
		{
			[ReadOnly] public NativeArray<Entity> TargetEntities;
			[ReadOnly] public NativeArray<Ship> TargetShips;
			[ReadOnly] public NativeArray<Translation> TargetTranslations;

			public void Execute(ref FindTarget findTarget, [ReadOnly] ref Ship ship, [ReadOnly] ref Translation translation)
			{
				var currentClosestDistanceSq = 0f;
				for (var j = 0; j < TargetEntities.Length; j++)
				{
					if (ship.FleetID != TargetShips[j].FleetID)
					{
						var distanceSq = math.distancesq(TargetTranslations[j].Value, translation.Value);
						if (findTarget.FoundEntity == Entity.Null || distanceSq < currentClosestDistanceSq)
						{
							currentClosestDistanceSq = distanceSq;
							findTarget.FoundEntity = TargetEntities[j];
						}
					}
				}
			}
		}

		[ExcludeComponent(typeof(Target))]
		public struct AddTargetsJob : IJobForEachWithEntity<FindTarget>
		{
			[WriteOnly]
			public EntityCommandBuffer.Concurrent CommandBuffer;

			public void Execute(Entity entity, int index, ref FindTarget findTarget)
			{
				if (findTarget.FoundEntity != Entity.Null)
				{
					Debug.Log("Add target");
					CommandBuffer.AddComponent(index, entity, new Target() { TargetEntity = findTarget.FoundEntity });
					CommandBuffer.RemoveComponent(index, entity, typeof(FindTarget));
				}
			}
		}
	}

	//[UpdateInGroup(typeof(BattleLogicSystemGroup))]
	//[UpdateAfter(typeof(RandomizedInitialDeploymentSystem))]
	//public class FindTargetSystem : ComponentSystem
	//{
	//	protected override void OnUpdate()
	//	{
	//		Entities.WithAll<FindTarget>().WithNone<Target>().ForEach((Entity entity, SharedFleetGrouping fleet, ref Translation translation, ref Rotation rotation) =>
	//		{
	//			Entity targetEntity = Entity.Null;
	//			var currentClosestDistanceSq = 0f;
	//			var trans = translation;
	//			Entities.ForEach((Entity otherEntity, SharedFleetGrouping otherFleet, ref Ship ship, ref Translation otherTranslation) =>
	//			{
	//				if (fleet.ID != otherFleet.ID)
	//				{
	//					var distanceSq = math.distancesq(otherTranslation.Value, trans.Value);
	//					if (targetEntity == Entity.Null || distanceSq < currentClosestDistanceSq)
	//					{
	//						currentClosestDistanceSq = distanceSq;
	//						targetEntity = otherEntity;
	//					}
	//				}
	//			});

	//			if (targetEntity != Entity.Null)
	//			{
	//				PostUpdateCommands.RemoveComponent(entity, typeof(FindTarget));
	//				PostUpdateCommands.AddComponent(entity, new Target() { TargetEntity = targetEntity });
	//			}
	//		});
	//	}
	//}
}