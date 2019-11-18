using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH.Data
{
	[CreateAssetMenu(fileName = "ship_view", menuName = "GH/Ship View")]
	public class ShipViewData : ScriptableObject
	{
		public ShipSpecsData ShipSpecs;

		public GameObject ViewPrefab;

        public MeshRenderer Mesh;

        public MeshFilter Filter;
	}
}