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
		public ShipSpecsData ShipSpecs;

		public EFactions Faction;

        public int FleetID;

		public int ShipCount;

		void Start()
		{
			var entityManager = World.Active.EntityManager.World.EntityManager;
			var entity = entityManager.CreateEntity();
            var spawnFleet = new SpawnFleet()
            {
                ShipID = ShipSpecs.ID,
                Faction = Faction,
                FleetID = FleetID,
                ShipCount = ShipCount,
                TopSpeed = ShipSpecs.TopSpeed,
                RotationSpeed = ShipSpecs.RotationSpeed,
                Acceleration = 0f,
                Deceleration = 0f
            };

            entityManager.AddComponentData(entity, spawnFleet);
		}
	}
}