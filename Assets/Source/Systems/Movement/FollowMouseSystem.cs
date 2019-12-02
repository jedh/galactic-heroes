using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class FollowMouseSystem : ComponentSystem
    {
        private const float k_DistanceFromTarget = 0.001f;

        protected override void OnUpdate()
        {
            Entities.WithAll<FollowMouse>().ForEach((Entity entity, ref MovementStats stats) =>
            {
                if(Input.GetMouseButtonDown(0))
                {
                    float3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePosition.y = 0f;

                    PostUpdateCommands.AddComponent<DeployToPosition>(entity);
                    PostUpdateCommands.SetComponent(entity, new DeployToPosition() { Position = mousePosition, ShouldStop = !stats.DoesSwarm });
                }
            });
        }
    }
}