using GH.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Systems
{
    public class FollowMouseSystem : ComponentSystem
    {
        private const float k_DistanceFromTarget = 0.001f;

        protected override void OnUpdate()
        {
            Entities.WithAll<FollowMouse>().ForEach((Entity entity) =>
            {
                if(Input.GetMouseButtonDown(0))
                {
                    float3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePosition.y = 0f;

                    PostUpdateCommands.AddComponent<DeployToPosition>(entity);
                    PostUpdateCommands.SetComponent(entity, new DeployToPosition() { Position = mousePosition });
                }
            });
        }
    }
}