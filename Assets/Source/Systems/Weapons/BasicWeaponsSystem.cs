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
    public class BasicWeaponsSystem : ComponentSystem
    {
        private EntityCommandBufferSystem m_EntityCommandBufferSystem;
        private EntityQuery m_TargetShipsQuery;

        protected override void OnCreate()
        {
            var targetShipsQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<Ship>(), ComponentType.ReadOnly<Translation>() }
            };
            m_TargetShipsQuery = GetEntityQuery(targetShipsQueryDesc);  // no need to dispose of this, unity will get mad if you try. 

            m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            m_EntityCommandBufferSystem = null;
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            var targetEntities = m_TargetShipsQuery.ToEntityArray(Allocator.TempJob);
            var targetShips = m_TargetShipsQuery.ToComponentDataArray<Ship>(Allocator.TempJob);
            var targetTranslations = m_TargetShipsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            try
            {
                Entities.ForEach((Entity entity, ref Ship ship, ref Target target) =>
                {
                    bool foundTarget = false;
                    for (int i = 0; i < targetEntities.Length; i++)
                    {
                        var targetEntity = targetEntities[i];
                        if (targetEntity.Index == target.TargetEntity.Index && targetEntity.Version == target.TargetEntity.Version)
                        {
                            // do the damage
                                // check distance
                                // calculate damage

                            foundTarget = true;
                            break;
                        }
                    }

                    if(!foundTarget)
                    {
                        PostUpdateCommands.RemoveComponent<Target>(entity); // target no longer exists, remove.
                    }
                });
            }
            finally
            {
                targetEntities.Dispose();
                targetShips.Dispose();
                targetTranslations.Dispose();
            }
        }
    }  
 }
