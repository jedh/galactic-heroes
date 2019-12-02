using GH.Components;
using GH.SystemGroups;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class DetermineRangedMoveTargetSystem : ComponentSystem
    {
        [ReadOnly]
        public ComponentDataFromEntity<Translation> TranslationData;

        protected override void OnUpdate()
        {
            TranslationData = GetComponentDataFromEntity<Translation>(true);

            Entities.WithNone<DeployToPosition>().ForEach((Entity e, ref RangedMovement rangedMovement, ref Target target, ref Translation translation, ref WeaponStats weaponStats) =>
            {
                if (TranslationData.Exists(target.TargetEntity))
                {
                    var targetPosition = TranslationData[target.TargetEntity];
                    var distanceSq = math.distancesq(targetPosition.Value, translation.Value);
                    if (distanceSq > weaponStats.OptimalRange)
                    {
                        PostUpdateCommands.AddComponent(e, new DeployToPosition() { Position = targetPosition.Value });
                    }
                }
            });

            Entities.ForEach((Entity e, ref RangedMovement rangedMovement, ref Target target, ref DeployToPosition deployToPosition, ref Translation translation, ref WeaponStats weaponStats) =>
            {
                if (TranslationData.Exists(target.TargetEntity))
                {
                    var targetPosition = TranslationData[target.TargetEntity];
                    var distanceSq = math.distancesq(targetPosition.Value, translation.Value);
                    if (distanceSq > weaponStats.OptimalRange)
                    {
                        PostUpdateCommands.SetComponent(e, new DeployToPosition() { Position = targetPosition.Value, ShouldStop = true });
                    } 
                }
            });
        }
    }
}