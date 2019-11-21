using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH.Data
{
    [CreateAssetMenu(fileName = "ship_view_database", menuName = "GH/Ship View Database")]
    public class ShipViewDatabase : ScriptableObject
    {
        public List<ShipViewData> ShipViews;
    }
}
