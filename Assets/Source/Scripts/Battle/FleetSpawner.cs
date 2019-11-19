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

		public int ShipCount;

		void Start()
		{
			var entityManager = World.Active.EntityManager.World.EntityManager;
			var entity = entityManager.CreateEntity();

			NativeArray<Entity> n = new NativeArray<Entity>(ShipCount, Allocator.Temp);
			entityManager.Instantiate(entity, n);

			var spawnShip = new SpawnShip()
			{
				ShipID = ShipSpecs.ID,
				Faction = Faction,
				Position = Vector3.zero,
				Rotation = transform.rotation,
				TopSpeed = ShipSpecs.TopSpeed,
				RotationSpeed = ShipSpecs.RotationSpeed,
				Acceleration = 0f,
				Deceleration = 0f
			};


			for (var i = 0; i < ShipCount; i++)
			{
				var randomPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5F));
				spawnShip.Position = randomPosition;

				entityManager.AddComponentData(n[i], spawnShip);
			}

			n.Dispose();
		}
	}
}