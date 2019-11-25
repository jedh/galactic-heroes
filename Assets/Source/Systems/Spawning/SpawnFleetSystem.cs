using GH.Components;
using GH.SystemGroups;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleSetupSystemGroup))]
	public class SpawnFleetSystem : ComponentSystem
	{
		Dictionary<int, SharedFleetGrouping> m_SharedFleetGroupingMap;

		protected override void OnCreate()
		{
			m_SharedFleetGroupingMap = new Dictionary<int, SharedFleetGrouping>();
		}

		protected override void OnUpdate()
		{
			Entities.WithNone<SpawnEntityState>().ForEach((Entity entity, ref SpawnFleet spawnFleet) =>
			{
				SharedFleetGrouping sharedFleetGrouping;
				if (!m_SharedFleetGroupingMap.TryGetValue(spawnFleet.FleetID, out sharedFleetGrouping))
				{
					sharedFleetGrouping = new SharedFleetGrouping() { ID = spawnFleet.FleetID };
					m_SharedFleetGroupingMap.Add(spawnFleet.FleetID, sharedFleetGrouping);
				}

				var shipEntities = new NativeArray<Entity>(spawnFleet.ShipCount, Allocator.Temp);
				var ent = EntityManager.CreateEntity();
				EntityManager.Instantiate(ent, shipEntities);

				var ship = new Ship() { ID = spawnFleet.ShipID };
				var deploy = new InitialDeploy() { FleetID = spawnFleet.FleetID };
				var translation = new Translation() { Value = new float3(100, 0, 100) };
				var rotation = new Rotation() { Value = quaternion.identity };
				var moveSpeed = default(Velocity);
				var rotateSpeed = default(AngularVelocity);
				var localToWorld = new LocalToWorld();
				//var movementType = new TurnThenMove();
				var movementType = new TurnAndMove();
				var movementStats = new MovementStats()
				{
					TopSpeed = spawnFleet.TopSpeed,
					Acceleration = spawnFleet.Acceleration,
					Deceleration = spawnFleet.Deceleration,
					RotationSpeed = spawnFleet.RotationSpeed
				};

				var followMouse = new FollowMouse();

				for (var i = 0; i < spawnFleet.ShipCount; i++)
				{
					ship.InstanceID = i;
					PostUpdateCommands.AddComponent(shipEntities[i], ship);
					PostUpdateCommands.AddComponent(shipEntities[i], translation);
					PostUpdateCommands.AddComponent(shipEntities[i], rotation);
					PostUpdateCommands.AddComponent(shipEntities[i], deploy);
					PostUpdateCommands.AddComponent(shipEntities[i], moveSpeed);
					PostUpdateCommands.AddComponent(shipEntities[i], rotateSpeed);
					PostUpdateCommands.AddComponent(shipEntities[i], localToWorld);
					PostUpdateCommands.AddComponent(shipEntities[i], movementType);
					PostUpdateCommands.AddComponent(shipEntities[i], movementStats);
					PostUpdateCommands.AddComponent(shipEntities[i], followMouse);
					PostUpdateCommands.AddSharedComponent(shipEntities[i], sharedFleetGrouping);
				}

				shipEntities.Dispose();

				PostUpdateCommands.AddComponent(entity, default(SpawnEntityState));
				PostUpdateCommands.DestroyEntity(entity);
			});
		}
	}
}