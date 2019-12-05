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
    public class FindTargetSystem : JobComponentSystem
    {
        private EntityQuery m_ShipsQuery;

        private EntityQuery m_TargetShipsQuery;

        private EntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            var shipsQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(FindTarget), ComponentType.ReadOnly<Translation>(), typeof(SharedFleetGrouping) },
                None = new ComponentType[] { typeof(Target) }
            };
            m_ShipsQuery = GetEntityQuery(shipsQueryDesc);

            var targetShipsQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<Ship>(), ComponentType.ReadOnly<Translation>(), typeof(SharedFleetGrouping) }
            };
            m_TargetShipsQuery = GetEntityQuery(targetShipsQueryDesc);

            m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        public struct FindTargetCompleted
        {
            public int JobIndex;
            public Entity TargetingShip;
            public Entity FoundShip;
        }

        [BurstCompile]
        public struct FindTargetJob : IJobChunk
        {
            [WriteOnly] public NativeQueue<FindTargetCompleted>.ParallelWriter CompletedQueue;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
            [ReadOnly] public NativeArray<Translation> TargetTranslations;
            [ReadOnly] public NativeArray<Entity> TargetEntities;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var entities = chunk.GetNativeArray(EntityType);
                var chunkTranslations = chunk.GetNativeArray(TranslationType);
                for (var i = 0; i < chunk.Count; i++)
                {
                    var currentClosestDistanceSq = 0f;
                    Entity targetEntity = Entity.Null;
                    var translation = chunkTranslations[i];
                    for (var j = 0; j < TargetEntities.Length; j++)
                    {
                        var distanceSq = math.distancesq(TargetTranslations[j].Value, translation.Value);
                        if (targetEntity == Entity.Null || distanceSq < currentClosestDistanceSq)
                        {
                            currentClosestDistanceSq = distanceSq;
                            targetEntity = TargetEntities[j];
                        }
                    }

                    if (targetEntity != Entity.Null)
                    {
                        CompletedQueue.Enqueue(new FindTargetCompleted() { FoundShip = targetEntity, TargetingShip = entities[i] });
                    }
                }
            }
        }

        public struct AddTargetsJob : IJob
        {
            public NativeQueue<FindTargetCompleted> CompletedQueue;

            [WriteOnly]
            public EntityCommandBuffer.Concurrent CommandBuffer;

            public void Execute()
            {
                while (CompletedQueue.TryDequeue(out FindTargetCompleted f))
                {
                    CommandBuffer.AddComponent(f.JobIndex, f.TargetingShip, new Target() { TargetEntity = f.FoundShip });
                    CommandBuffer.RemoveComponent(f.JobIndex, f.TargetingShip, typeof(FindTarget));
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var completedQueue = new NativeQueue<FindTargetCompleted>(Allocator.TempJob);
            var entityType = GetArchetypeChunkEntityType();
            var translationType = GetArchetypeChunkComponentType<Translation>(true);

            // Find targets for fleet 0.
            m_ShipsQuery.SetFilter(new SharedFleetGrouping() { ID = 0 });
            m_TargetShipsQuery.SetFilter(new SharedFleetGrouping() { ID = 1 });
            var targetTranslations1 = m_TargetShipsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var targetEntities1 = m_TargetShipsQuery.ToEntityArray(Allocator.TempJob);
            var findTargetJobHandle1 = new FindTargetJob()
            {
                CompletedQueue = completedQueue.AsParallelWriter(),
                EntityType = entityType,
                TranslationType = translationType,
                TargetTranslations = targetTranslations1,
                TargetEntities = targetEntities1
            }.Schedule(m_ShipsQuery, inputDeps);
            findTargetJobHandle1.Complete();

            // Find targets for fleet 1.
            m_ShipsQuery.SetFilter(new SharedFleetGrouping() { ID = 1 });
            m_TargetShipsQuery.SetFilter(new SharedFleetGrouping() { ID = 0 });
            var targetTranslations2 = m_TargetShipsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var targetEntities2 = m_TargetShipsQuery.ToEntityArray(Allocator.TempJob);
            var findTargetJobHandle2 = new FindTargetJob()
            {
                CompletedQueue = completedQueue.AsParallelWriter(),
                EntityType = entityType,
                TranslationType = translationType,
                TargetTranslations = targetTranslations2,
                TargetEntities = targetEntities2
            }.Schedule(m_ShipsQuery, inputDeps);
            findTargetJobHandle2.Complete();

            var targetDependency = JobHandle.CombineDependencies(findTargetJobHandle1, findTargetJobHandle2);

            var addTargetsJob = new AddTargetsJob()
            {
                CompletedQueue = completedQueue,
                CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(targetDependency);
            addTargetsJob.Complete();

            var queueCleanupJobHandle = completedQueue.Dispose(addTargetsJob);

            m_EntityCommandBufferSystem.AddJobHandleForProducer(queueCleanupJobHandle);

            targetTranslations1.Dispose();
            targetEntities1.Dispose();
            targetTranslations2.Dispose();
            targetEntities2.Dispose();

            return queueCleanupJobHandle;
        }
    }

    //[UpdateInGroup(typeof(BattleLogicSystemGroup))]
    //public class FindTargetSystem : ComponentSystem
    //{
    //    protected override void OnUpdate()
    //    {
    //        Entities.WithAll<FindTarget>().WithNone<Target>().ForEach((Entity entity, SharedFleetGrouping fleet, ref Translation translation, ref Rotation rotation) =>
    //        {
    //            Entity targetEntity = Entity.Null;
    //            var currentClosestDistanceSq = 0f;
    //            var trans = translation;
    //            Entities.ForEach((Entity otherEntity, SharedFleetGrouping otherFleet, ref Ship ship, ref Translation otherTranslation) =>
    //            {
    //                if (fleet.ID != otherFleet.ID)
    //                {
    //                    var distanceSq = math.distancesq(otherTranslation.Value, trans.Value);
    //                    if (targetEntity == Entity.Null || distanceSq < currentClosestDistanceSq)
    //                    {
    //                        currentClosestDistanceSq = distanceSq;
    //                        targetEntity = otherEntity;
    //                    }
    //                }
    //            });

    //            if (targetEntity != Entity.Null)
    //            {
    //                PostUpdateCommands.RemoveComponent(entity, typeof(FindTarget));
    //                PostUpdateCommands.AddComponent(entity, new Target() { TargetEntity = targetEntity });
    //            }
    //        });
    //    }
    //}
}