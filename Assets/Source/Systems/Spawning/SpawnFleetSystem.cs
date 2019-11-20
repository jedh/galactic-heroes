using GH.Components;
using GH.SystemGroups;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleSetupSystemGroup))]
    public class SpawnFleetSystem : ComponentSystem
    {
        Random m_Random;

        protected override void OnCreate()
        {
            m_Random = new Random((uint)System.DateTime.Now.Ticks);
        }

        protected override void OnUpdate()
        {
            Entities.WithNone<SpawnEntityState>().ForEach((Entity entity, ref SpawnFleet spawnFleet) =>
            {
                var shipEntities = new NativeArray<Entity>(spawnFleet.ShipCount, Allocator.Temp);
                var ent = EntityManager.CreateEntity();
                EntityManager.Instantiate(ent, shipEntities);

                var ship = new Ship() { ID = spawnFleet.ShipID };
                var translation = new Translation() { Value = float3.zero };
                var rotation = default(Rotation);
                var moveSpeed = default(MoveSpeed);
                var rotateSpeed = default(RotateSpeed);
                var localToWorld = new LocalToWorld();
                var movementStats = new MovementStats()
                {
                    TopSpeed = spawnFleet.TopSpeed,
                    Acceleration = spawnFleet.Acceleration,
                    Deceleration = spawnFleet.Deceleration,
                    RotationSpeed = spawnFleet.RotationSpeed
                };

                for (var i = 0; i < spawnFleet.ShipCount; i++)
                {
                    var randomPosition = m_Random.NextFloat3(-5f, 5f);
                    randomPosition.y = 0f;
                    translation.Value = randomPosition;

                    ship.InstanceID = i;
                    PostUpdateCommands.AddComponent(shipEntities[i], ship);
                    PostUpdateCommands.AddComponent(shipEntities[i], translation);
                    PostUpdateCommands.AddComponent(shipEntities[i], rotation);
                    PostUpdateCommands.AddComponent(shipEntities[i], moveSpeed);
                    PostUpdateCommands.AddComponent(shipEntities[i], rotateSpeed);
                    PostUpdateCommands.AddComponent(shipEntities[i], localToWorld);
                    PostUpdateCommands.AddComponent(shipEntities[i], movementStats);
                }

                shipEntities.Dispose();

                PostUpdateCommands.AddComponent(entity, default(SpawnEntityState));
                PostUpdateCommands.DestroyEntity(entity);
            });
        }
    }
}