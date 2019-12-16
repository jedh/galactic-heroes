using GH.Components;
using GH.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class BasicWeaponsSystem : ComponentSystem
    {
        private const float k_ChipDmaage = 0.1f;
        private EntityQuery m_TargetShipsQuery;

        protected override void OnCreate()
        {
            var targetShipsQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly<Ship>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Rotation>() }
            };
            m_TargetShipsQuery = GetEntityQuery(targetShipsQueryDesc);  // no need to dispose of this, unity will get mad if you try. 

            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            var targetEntities = m_TargetShipsQuery.ToEntityArray(Allocator.TempJob);
            var targetShips = m_TargetShipsQuery.ToComponentDataArray<Ship>(Allocator.TempJob);
            var targetTranslations = m_TargetShipsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var targetRotations = m_TargetShipsQuery.ToComponentDataArray<Rotation>(Allocator.TempJob);

            try
            {
                Entities.ForEach((Entity entity, ref Target target, ref WeaponStats weaponStats, ref Translation translation) =>
                {
                    bool foundTarget = false;
                    for (int i = 0; i < targetEntities.Length; i++)
                    {
                        var targetEntity = targetEntities[i];
                        if (targetEntity.Index == target.TargetEntity.Index && targetEntity.Version == target.TargetEntity.Version)
                        {
                            // check distance
                            float3 targetPosition = targetTranslations[i].Value;

                            float minSq = weaponStats.MinRange * weaponStats.MinRange;
                            float maxSq = weaponStats.MaxRange * weaponStats.MaxRange;
                            float3 dir = translation.Value - targetPosition;
                            float distanceSq = math.dot(dir, dir);

                            if (distanceSq <= maxSq && distanceSq >= minSq)
                            {
                                // calculate damage
                                Ship ship = targetShips[i];
                                quaternion targetRotation = targetRotations[i].Value;

                                float3 targetForward = math.forward(targetRotation);

                                // simple, assumes a turret, no firing arc.
                                float costheta = math.dot(math.normalize(dir), math.normalize(targetForward));

                                // step gives us (1,1,1), (0,1,1) or (0,0,1), lengthsq gives us 3, 2 or 1, thus 3 - lengthsq gives us index 0, 1 or 2 respectivley.
                                int hullIndex = (int)(3f - math.lengthsq(math.step(ship.HullArc, costheta)));    // assumes that Arc looks something like (0.7, -0.2, -1)
                                int shieldIndex =  (int)(3f - math.lengthsq(math.step(ship.ShieldArc, costheta)));   // where front arc goes from [1, 0.7] the mid arc (0.7, -0.2] and rear arc (-0.2, -1]

                                float hull = ship.Hull[math.min(hullIndex, 2)];
                                float shield = ship.Shield[math.min(shieldIndex, 2)];

                                float defense = hull + shield;
                                float damage = math.max(k_ChipDmaage, weaponStats.Damage - defense);

                                ship.HP = ship.HP - damage;
                                
                                // apply the damage back to the entity...somehow...

                                if (ship.HP <= 0f)
                                {
                                    // he ded.
                                    PostUpdateCommands.DestroyEntity(targetEntity);
                                }
                            }

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
                targetRotations.Dispose();
            }
        }
    }  
 }
