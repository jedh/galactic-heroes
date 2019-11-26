using System.Collections;
using System.Collections.Generic;
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

		public float Hull;

		public float Shields;

		public float ShieldRechargeRate;

		public int CrewCount;

        [Range(1, 100)]
        public int SquadSize = 1;

		private void Awake()
		{
			ID = GetHashCode();
		}
	}
}