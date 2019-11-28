using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class FaceTargetSystem : ComponentSystem
    {
        private const float k_Tolerance = 0.9999f;

        protected override void OnUpdate()
        {
            Entities.WithNone<RotateTowardsPosition>()
                .ForEach((Entity entity, ref FaceTarget target, ref Rotation rotation, ref Translation translation) =>
                {
                    float3 toPos = math.normalize(target.Value - translation.Value);
                    float3 forward = math.normalize(math.forward(rotation.Value));

                    float costheta = math.dot(toPos, forward);

                    if (costheta <= k_Tolerance)
                    {
                        PostUpdateCommands.AddComponent<RotateTowardsPosition>(entity);
                        PostUpdateCommands.SetComponent(entity, new RotateTowardsPosition() { Value = target.Value });

                        return;
                    }
                });
        }
    }
}