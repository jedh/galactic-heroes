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

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleLogicSystemGroup))]
	public class FindTargetSystem : JobComponentSystem
	{
        private EntityQuery m_ShipsQuery;

        private EntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            var queryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(FindTarget), ComponentType.ReadOnly<Translation>(), typeof(SharedFleetGrouping) },
                None = new ComponentType[] {typeof(Target)}
            };

            m_ShipsQuery = GetEntityQuery(queryDesc);

            m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        public struct FindTargetCompleted
        {
            public int JobIndex;
            public Entity TargetingShip;
            public Entity FoundShip;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var completedQueue = new NativeQueue<FindTargetCompleted>(Allocator.TempJob);

            var findTargetJobHandle = new FindTargetJob()
            {
                CompletedQueue = completedQueue.AsParallelWriter()
            }.Schedule(m_ShipsQuery, inputDeps);

            var addTargetsJob = new AddTargetsJob()
            {
                CompletedQueue = completedQueue,
                CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(m_ShipsQuery, inputDeps);

            var queueCleanupJobHandle = completedQueue.Dispose(addTargetsJob);

            m_EntityCommandBufferSystem.AddJobHandleForProducer(queueCleanupJobHandle);

            return queueCleanupJobHandle;
        }

        public struct FindTargetJob : IJobChunk
        {
            [WriteOnly]
            public NativeQueue<FindTargetCompleted>.ParallelWriter CompletedQueue;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                throw new System.NotImplementedException();
            }
        }

        public struct AddTargetsJob : IJobChunk
        {
            public NativeQueue<FindTargetCompleted> CompletedQueue;

            [WriteOnly]
            public EntityCommandBuffer.Concurrent CommandBuffer;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                throw new System.NotImplementedException();
            }
        }

  //      protected void OnUpdate()
		//{
		//	Entities.WithAll<FindTarget>().WithNone<Target>().ForEach((Entity entity, SharedFleetGrouping fleet, ref Translation translation, ref Rotation rotation) =>
		//	{
		//		Entity targetEntity = Entity.Null;
		//		var currentClosestDistanceSq = 0f;
		//		var trans = translation;
		//		Entities.ForEach((Entity otherEntity, SharedFleetGrouping otherFleet, ref Ship ship, ref Translation otherTranslation) =>
		//		{
		//			if (fleet.ID != otherFleet.ID)
		//			{
		//				var distanceSq = math.distancesq(otherTranslation.Value, trans.Value);
		//				if (targetEntity == Entity.Null || distanceSq < currentClosestDistanceSq)
		//				{
		//					currentClosestDistanceSq = distanceSq;
		//					targetEntity = otherEntity;
		//				}
		//			}
		//		});

		//		if (targetEntity != Entity.Null)
		//		{
		//			PostUpdateCommands.RemoveComponent(entity, typeof(FindTarget));
		//			PostUpdateCommands.AddComponent(entity, new Target() { TargetEntity = targetEntity });
		//		}
		//	});
		//}
    }
}