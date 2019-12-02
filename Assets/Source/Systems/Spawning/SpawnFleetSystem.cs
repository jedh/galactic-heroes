using GH.Components;
using GH.SystemGroups;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

				var ship = new Ship() { ID = spawnFleet.ShipID };
				var deploy = new InitialDeploy() { FleetID = spawnFleet.FleetID };
				var translation = new Translation() { Value = new float3(100, 0, 100) };
				var rotation = new Rotation() { Value = quaternion.identity };
				var moveSpeed = default(Velocity);
				var rotateSpeed = default(AngularVelocity);
				var localToWorld = new LocalToWorld();
				var findTarget = new FindTarget();
				var movementStats = new MovementStats()
				{
					TopSpeed = spawnFleet.TopSpeed,
					Acceleration = spawnFleet.Acceleration,
					Deceleration = spawnFleet.Deceleration,
					RotationSpeed = spawnFleet.RotationSpeed,
					ThrustTolerance = spawnFleet.ThrustTolerance,
					MaxSpeedToTurn = spawnFleet.MaxSpeedToTurn,
					DoesSwarm = spawnFleet.DoesSwarm
				};

				var shipEntities = new NativeArray<Entity>(spawnFleet.ShipCount * spawnFleet.SquadSize, Allocator.Temp);

                var ent = EntityManager.CreateEntity();
                {
                    // do this once only.
                    EntityManager.AddComponent<Ship>(ent);
                    EntityManager.AddComponent<MovementStats>(ent);
                    EntityManager.AddComponent<LocalToWorld>(ent);
                    EntityManager.AddComponent<Translation>(ent);
                    EntityManager.AddComponent<Rotation>(ent);
                    EntityManager.AddComponent<Velocity>(ent);
                    EntityManager.AddComponent<AngularVelocity>(ent);
                    EntityManager.AddComponent<FindTarget>(ent);
                    EntityManager.AddComponent<InitialDeploy>(ent);

                    EntityManager.AddSharedComponentData(ent, sharedFleetGrouping);

                    EntityManager.SetComponentData(ent, ship);
                    EntityManager.SetComponentData(ent, movementStats);
                    EntityManager.SetComponentData(ent, localToWorld);
                    EntityManager.SetComponentData(ent, translation);
                    EntityManager.SetComponentData(ent, rotation);
                    EntityManager.SetComponentData(ent, moveSpeed);
                    EntityManager.SetComponentData(ent, rotateSpeed);
                    EntityManager.SetComponentData(ent, deploy);
                }

                // apply the above to all spawned entities.
                EntityManager.Instantiate(ent, shipEntities);

				var index = 0;
				for (var i = 0; i < spawnFleet.ShipCount; i++)
				{
					SharedSquadGrouping sharedSquadGrouping = new SharedSquadGrouping();
					if (spawnFleet.SquadSize > 1)
					{
						sharedSquadGrouping.ID = GetHashCode();
					}

					for (var j = 0; j < spawnFleet.SquadSize; j++)
					{
						ship.InstanceID = index;

						PostUpdateCommands.SetComponent(shipEntities[index], ship);

						if (spawnFleet.SquadSize > 1)
						{
							PostUpdateCommands.AddSharedComponent(shipEntities[index], sharedSquadGrouping);
						}

						index++;
					}
				}

				shipEntities.Dispose();

				PostUpdateCommands.AddComponent(entity, default(SpawnEntityState));
				PostUpdateCommands.DestroyEntity(entity);
			});
		}
	}
}