using GH.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
	public List<ShipViewData> ShipViewDataAssets;

    public Dictionary<int, ShipViewData> ShipViewDataMap { get; private set; } = new Dictionary<int, ShipViewData>();

    private void Awake()
    {
        foreach (var shipViewData in ShipViewDataAssets)
        {
            if (!ShipViewDataMap.ContainsKey(shipViewData.ShipSpecs.ID))
            {
                ShipViewDataMap.Add(shipViewData.ShipSpecs.ID, shipViewData);
            }
        }
    }
}
