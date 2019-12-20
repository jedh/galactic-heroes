using GH.Components;
using GH.SystemGroups;
using Unity.Entities;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class CleanupTargetsSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref Target target) =>
            {
                if (!EntityManager.Exists(target.TargetEntity))
                {
                    PostUpdateCommands.RemoveComponent<Target>(entity);
                    PostUpdateCommands.AddComponent<FindTarget>(entity);
                }
            });
        }
    }
}