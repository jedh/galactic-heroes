using GH.Components;
using GH.Data;
using GH.Enums;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GH.Scripts
{
	public class FleetSpawner : MonoBehaviour
	{
        public FleetRosterData FleetRoster;

		void Start()
		{
			var entityManager = World.Active.EntityManager.World.EntityManager;
            for (int i = 0; i < FleetRoster.ShipTypeEntries.Count; i++)
            {
                var shipTypeEntry = FleetRoster.ShipTypeEntries[i];
                var entity = entityManager.CreateEntity();

                int squadID = 0;
                if (shipTypeEntry.ShipSpecs.SquadSize > 1)
                {
                    // Only assign a squad ID if there is more than one ship in the squad.
                    squadID = GetHashCode();
                }

                var spawnFleet = new SpawnFleet()
                {
                    ShipID = shipTypeEntry.ShipSpecs.ID,
                    Faction = FleetRoster.Faction,
                    FleetID = FleetRoster.FleetID,
                    ShipCount = shipTypeEntry.Count,
                    TopSpeed = shipTypeEntry.ShipSpecs.TopSpeed,
                    RotationSpeed = shipTypeEntry.ShipSpecs.RotationSpeed,
                    Acceleration = shipTypeEntry.ShipSpecs.Acceleration,
                    Deceleration = shipTypeEntry.ShipSpecs.Deceleration,
                    SquadSize = shipTypeEntry.ShipSpecs.SquadSize,
                    ThrustTolerance = shipTypeEntry.ShipSpecs.ThrustTolerance,
                    MaxSpeedToTurn = shipTypeEntry.ShipSpecs.MaxSpeedToTurn,
                    DoesSwarm = shipTypeEntry.ShipSpecs.DoesSwarm
                };

                entityManager.AddComponentData(entity, spawnFleet);
            }
		}
	}
}