using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH.Data
{
	[CreateAssetMenu(fileName = "ship_view", menuName = "GH/Ship View")]
	public class ShipViewData : ScriptableObject
	{
		public string NameID;

		public GameObject ViewPrefab;
	}
}