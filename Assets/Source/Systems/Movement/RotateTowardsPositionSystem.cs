using GH.Components;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Systems
{
    public class RotateTowardsPositionSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref RotateTowardsPosition target, ref MovementStats stats, ref Translation translation) =>
            {
                // logic
            });
        }
    }
}