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
        private const float k_ChipDamage = 0.1f;
        protected override void OnUpdate()
        {
            var ShipData = GetComponentDataFromEntity<Ship>(false);
            var TranslationData = GetComponentDataFromEntity<Translation>(false);
            var RotationData = GetComponentDataFromEntity<Rotation>(false);

            Entities.ForEach((Entity entity, ref Target target, ref WeaponStats weaponStats, ref Translation translation) =>
            {
                // check distance
                float3 targetPosition = TranslationData[target.TargetEntity].Value;

                float minSq = weaponStats.MinRange * weaponStats.MinRange;
                float maxSq = weaponStats.MaxRange * weaponStats.MaxRange;
                float3 dir = translation.Value - targetPosition;
                float distanceSq = math.dot(dir, dir);

                if (distanceSq <= maxSq && distanceSq >= minSq)
                {
                    // calculate damage
                    Ship ship = ShipData[target.TargetEntity];
                    quaternion targetRotation = RotationData[target.TargetEntity].Value;

                    float3 targetForward = math.forward(targetRotation);

                    // simple, assumes a turret, no firing arc.
                    float costheta = math.dot(math.normalize(dir), math.normalize(targetForward));

                    // step gives us (1,1,1), (0,1,1) or (0,0,1), lengthsq gives us 3, 2 or 1, thus 3 - lengthsq gives us index 0, 1 or 2 respectivley.
                    int hullIndex = (int)(3f - math.lengthsq(math.step(ship.HullArc, costheta)));    // assumes that Arc looks something like (0.7, -0.2, -1)
                    int shieldIndex = (int)(3f - math.lengthsq(math.step(ship.ShieldArc, costheta)));   // where front arc goes from [1, 0.7] the mid arc (0.7, -0.2] and rear arc (-0.2, -1]

                    float hull = ship.Hull[math.min(hullIndex, 2)];
                    float shield = ship.Shield[math.min(shieldIndex, 2)];

                    float defense = hull + shield;
                    float damage = math.max(k_ChipDamage, weaponStats.Damage - defense);

                    ship.HP = ship.HP - damage;

                    ShipData[target.TargetEntity] = ship;

                    if (ship.HP <= 0f)
                    {
                        // he ded.
                        //Debug.Log($"killed entity: {target.TargetEntity}");
                        PostUpdateCommands.DestroyEntity(target.TargetEntity);
                    }
                }
            });
        }
    }  
 }
