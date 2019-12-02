using GH.Components;
using GH.SystemGroups;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class DetermineSwarmMoveTargetSystem : ComponentSystem
    {
        [ReadOnly]
        public ComponentDataFromEntity<Translation> TranslationData;

        protected override void OnUpdate()
        {
            TranslationData = GetComponentDataFromEntity<Translation>(true);

            Entities.WithAll<SwarmMovement>().WithNone<DeployToPosition>().ForEach((Entity e, ref Target target, ref Translation translation) =>
            {
                //var targetEntity = target.TargetEntity;
                //Entities.ForEach((Entity otherEntity, ref Ship ship, ref Translation otherTranslation) =>
                //{
                //    if (targetEntity == otherEntity)
                //    {
                //        PostUpdateCommands.AddComponent(e, new DeployToPosition() { Position = otherTranslation.Value, ShouldStop = false });
                //    }
                //});

                if (TranslationData.Exists(target.TargetEntity))
                {
                    var position = TranslationData[target.TargetEntity];
                    PostUpdateCommands.AddComponent(e, new DeployToPosition() { Position = position.Value, ShouldStop = false });
                }
            });

            Entities.WithAll<SwarmMovement>().ForEach((Entity e, ref Target target, ref DeployToPosition deployToPosition, ref Translation translation) =>
            {
                //var targetEntity = target.TargetEntity;
                //Entities.ForEach((Entity otherEntity, ref Ship ship, ref Translation otherTranslation) =>
                //{
                //    if (targetEntity == otherEntity)
                //    {
                //        PostUpdateCommands.SetComponent(e, new DeployToPosition() { Position = otherTranslation.Value, ShouldStop = false });
                //    }
                //});

                if (TranslationData.Exists(target.TargetEntity))
                {
                    var position = TranslationData[target.TargetEntity];
                    PostUpdateCommands.SetComponent(e, new DeployToPosition() { Position = position.Value, ShouldStop = false });
                }
            });
        }
    }
}
