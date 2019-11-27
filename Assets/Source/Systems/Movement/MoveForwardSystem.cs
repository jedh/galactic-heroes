using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class MoveForwardSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref MoveForward moveForward, ref Rotation rotation, ref Velocity velocity) =>
            {
                float3 forwardVector = math.normalize(math.forward(rotation.Value));
                velocity.Value = forwardVector * moveForward.Speed;
            });
        }
    }
}