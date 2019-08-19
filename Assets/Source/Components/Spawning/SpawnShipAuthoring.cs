using GH.Data;
using GH.Enums;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Components
{
	[DisallowMultipleComponent]
	[RequiresEntityConversion]
	[Serializable]
	public class SpawnShipAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
	{
		public ShipSpecsData ShipData;

		public EFactions Faction;

		public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
		{
		}

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var spawnShip = new SpawnShip()
			{
				ShipID = ShipData.ID,
				Faction = Faction,
				Position = transform.position,
				Rotation = transform.rotation,
				TopSpeed = ShipData.TopSpeed,
				RotationSpeed = ShipData.RotationSpeed,
				Acceleration = 0f,
				Deceleration = 0f
			};

			dstManager.AddComponentData(entity, spawnShip);
		}
	}
}
