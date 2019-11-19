using GH.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Rendering;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
	public List<ShipSpecsData> ShipSpecsDataAssets;

	public List<ShipViewData> ShipViewDataAssets;

	public Dictionary<int, ShipSpecsData> ShipSpecsDataMap { get; private set; } = new Dictionary<int, ShipSpecsData>();

	public Dictionary<int, ShipViewData> ShipViewDataMap { get; private set; } = new Dictionary<int, ShipViewData>();

	private void Awake()
	{
		foreach (var shipSpecsData in ShipSpecsDataAssets)
		{
			if (!ShipSpecsDataMap.ContainsKey(shipSpecsData.ID))
			{
				ShipSpecsDataMap.Add(shipSpecsData.ID, shipSpecsData);
			}
		}

		foreach (var shipViewData in ShipViewDataAssets)
		{
			if (!ShipViewDataMap.ContainsKey(shipViewData.ShipSpecs.ID))
			{
				ShipViewDataMap.Add(shipViewData.ShipSpecs.ID, shipViewData);
			}
		}
	}
}
