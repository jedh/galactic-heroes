using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Data
{
	[CreateAssetMenu(fileName = "ship_specs", menuName = "GH/Ship Specs")]
	public class ShipSpecsData : ScriptableObject
	{
		public int ID;

		public string NameID;

		public float Acceleration;

		public float Deceleration;

		public float TopSpeed;

		public float RotationSpeed;

		public float ThrustTolerance;

		public float MaxSpeedToTurn;

		public bool DoesSwarm;

        public float3 HullArc;
        public float3 Hull;

        public float3 ShieldArc;
        public float3 Shield;

		public float ShieldRechargeRate;

		public int CrewCount;
        public float HP;

		[Range(1, 100)]
		public int SquadSize = 1;

		public WeaponSpecsData WeaponSpecs;

		private void Awake()
		{
			ID = GetHashCode();
		}
	}
}