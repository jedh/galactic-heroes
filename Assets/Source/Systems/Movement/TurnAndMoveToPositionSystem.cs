using GH.Components;
using GH.SystemGroups;
using Unity.Entities;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class TurnAndMoveToPositionSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<TurnAndMove>().ForEach((Entity entity, ref MoveToPosition moveTo) =>
            {
                PostUpdateCommands.RemoveComponent<MoveToPosition>(entity);
                
                PostUpdateCommands.AddComponent<TranslateToPosition>(entity);
                PostUpdateCommands.SetComponent(entity, new TranslateToPosition() { Value = moveTo.Value });  // start translating.

                PostUpdateCommands.AddComponent<RotateTowardsPosition>(entity);
                PostUpdateCommands.SetComponent(entity, new RotateTowardsPosition() { Value = moveTo.Value });  // start rotating.
            });
        }
    }
}