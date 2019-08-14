using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Transforms;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleSetupSystemGroup))]
    public class SpawnShipSystem : ComponentSystem
    {
        private int m_SpawnCounter;

        protected override void OnCreate()
        {
            m_SpawnCounter = 0;
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<SpawnEntity>().WithNone<SpawnEntityState>().ForEach((Entity entity, ref SpawnShip spawnShip) =>
            {
                var spawnedEntity = PostUpdateCommands.CreateEntity();
                PostUpdateCommands.AddComponent(spawnedEntity, new Translation() { Value = spawnShip.Position });
                PostUpdateCommands.AddComponent(spawnedEntity, new Rotation() { Value = spawnShip.Rotation });
                PostUpdateCommands.AddComponent(spawnedEntity, new Ship() { ID = spawnShip.ShipID, InstanceID = m_SpawnCounter });
                PostUpdateCommands.AddComponent(spawnedEntity, new View());
                PostUpdateCommands.AddSharedComponent(spawnedEntity, new SharedFaction() { Faction = spawnShip.Faction });                
                PostUpdateCommands.AddComponent(entity, default(SpawnEntityState));
                PostUpdateCommands.DestroyEntity(entity);

                m_SpawnCounter++;
            });
        }
    }
}