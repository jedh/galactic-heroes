using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH.Data
{
    [CreateAssetMenu(fileName = "ship_specs_database", menuName = "GH/Ship Specs Database")]
    public class ShipSpecsDatabase : ScriptableObject
    {
        public List<ShipSpecsData> ShipSpecs;
    }
}
