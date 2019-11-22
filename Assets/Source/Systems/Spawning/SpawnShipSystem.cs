using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

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
				// TODO: Look at moving all this definition code to an EntityArchetype. 
				var spawnedEntity = PostUpdateCommands.CreateEntity();
				PostUpdateCommands.AddComponent(spawnedEntity, new Translation() { Value = spawnShip.Position });
				PostUpdateCommands.AddComponent(spawnedEntity, new Rotation() { Value = spawnShip.Rotation });
				PostUpdateCommands.AddComponent(spawnedEntity, new Ship() { ID = spawnShip.ShipID, InstanceID = m_SpawnCounter });
				PostUpdateCommands.AddComponent(spawnedEntity, new Velocity() { Value = 0f });
				PostUpdateCommands.AddComponent(spawnedEntity, new AngularVelocity() { Value = 0f });
				PostUpdateCommands.AddComponent(spawnedEntity, new LocalToWorld());
				PostUpdateCommands.AddComponent(spawnedEntity, new MovementStats()
				{
					TopSpeed = spawnShip.TopSpeed,
					Acceleration = spawnShip.Acceleration,
					Deceleration = spawnShip.Deceleration,
					RotationSpeed = spawnShip.RotationSpeed
				});

				PostUpdateCommands.AddSharedComponent(spawnedEntity, new SharedFactionGrouping() { Faction = spawnShip.Faction });
				PostUpdateCommands.AddSharedComponent(spawnedEntity, new SharedFleetGrouping() { ID = spawnShip.FleetID });
				PostUpdateCommands.AddSharedComponent(spawnedEntity, new SharedGroupGrouping() { ID = spawnShip.GroupID });
				PostUpdateCommands.AddSharedComponent(spawnedEntity, new SharedSquadGrouping() { ID = spawnShip.SquadID });

				PostUpdateCommands.AddComponent(entity, default(SpawnEntityState));
				PostUpdateCommands.DestroyEntity(entity);

				m_SpawnCounter++;
			});
		}
	}
}