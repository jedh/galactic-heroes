using GH.Components;
using Unity.Entities;

namespace GH.Systems
{
    public class TurnThenMoveToPositionSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<TurnThenMove>().ForEach((Entity entity, ref MoveToPosition moveTo) =>
            {
                PostUpdateCommands.RemoveComponent<MoveToPosition>(entity);
                PostUpdateCommands.RemoveComponent<TranslateToPosition>(entity);    // if another move is active, override. maybe remove for drift?

                PostUpdateCommands.AddComponent<Moving>(entity);
                PostUpdateCommands.SetComponent(entity, new Moving() { Value = moveTo.Value });

                PostUpdateCommands.AddComponent<RotateTowardsPosition>(entity);
                PostUpdateCommands.SetComponent(entity, new RotateTowardsPosition() { Value = moveTo.Value });  // start rotating.
            });

            Entities.WithAll<TurnThenMove>().WithNone<RotateTowardsPosition>().ForEach((Entity entity, ref Moving moveTo) =>
            {
                PostUpdateCommands.AddComponent<TranslateToPosition>(entity);
                PostUpdateCommands.SetComponent(entity, new TranslateToPosition() { Value = moveTo.Value });  // start translating.
            });

            Entities.WithAll<TurnThenMove>().WithNone<TranslateToPosition>().WithNone<RotateTowardsPosition>().ForEach((Entity entity, ref Moving moveTo) =>
            {
                PostUpdateCommands.RemoveComponent<Moving>(entity);
            });
        }
    }
}