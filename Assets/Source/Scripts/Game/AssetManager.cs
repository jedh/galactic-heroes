using GH.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Rendering;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public ShipSpecsDatabase ShipSpecsDB;

    public ShipViewDatabase ShipViewDB;

	public Dictionary<int, ShipSpecsData> ShipSpecsDataMap { get; private set; } = new Dictionary<int, ShipSpecsData>();

	public Dictionary<int, ShipViewData> ShipViewDataMap { get; private set; } = new Dictionary<int, ShipViewData>();

	private void Awake()
	{
		foreach (var shipSpecsData in ShipSpecsDB.ShipSpecs)
		{
			if (!ShipSpecsDataMap.ContainsKey(shipSpecsData.ID))
			{
				ShipSpecsDataMap.Add(shipSpecsData.ID, shipSpecsData);
			}
		}

		foreach (var shipViewData in ShipViewDB.ShipViews)
		{
			if (!ShipViewDataMap.ContainsKey(shipViewData.ShipSpecs.ID))
			{
				ShipViewDataMap.Add(shipViewData.ShipSpecs.ID, shipViewData);
			}
		}
	}
}
