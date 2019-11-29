using GH.Components;
using GH.SystemGroups;
using Unity.Entities;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class TurnThenMoveToPositionSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<TurnThenMove>().ForEach((Entity entity, ref MoveToPosition moveTo) =>
            {
                PostUpdateCommands.RemoveComponent<MoveToPosition>(entity);
                PostUpdateCommands.RemoveComponent<TranslateToPosition>(entity);    // if another translation is active, stop it.

                // TODO: maybe zero velocity to prevent drift?

                PostUpdateCommands.AddComponent<DeployingTo>(entity);
                PostUpdateCommands.SetComponent(entity, new DeployingTo() { Position = moveTo.Value });

                PostUpdateCommands.AddComponent<RotateTowardsPosition>(entity);
                PostUpdateCommands.SetComponent(entity, new RotateTowardsPosition() { Value = moveTo.Value });  // start rotating.
            });

            Entities.WithAll<TurnThenMove>().WithNone<RotateTowardsPosition>().ForEach((Entity entity, ref DeployingTo moveTo) =>
            {
                PostUpdateCommands.RemoveComponent<DeployingTo>(entity);

                PostUpdateCommands.AddComponent<TranslateToPosition>(entity);
                PostUpdateCommands.SetComponent(entity, new TranslateToPosition() { Value = moveTo.Position });  // start translating.
            });
        }
    }
}