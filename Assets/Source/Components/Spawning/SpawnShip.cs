using GH.Enums;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace GH.Components
{
	[Serializable]
	public struct SpawnShip : IComponentData
	{
		public int ShipID;

		public EFactions Faction;

		public float3 Position;

		public quaternion Rotation;

		public float TopSpeed;

		public float RotationSpeed;

		public float Acceleration;

		public float Deceleration;

		public int FleetID;

		public int GroupID;

		public int SquadID;
	}
}
