using GH.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH.Data
{
    [Serializable]
    public class FleetShipTypesEntry
    {
        public ShipSpecsData ShipSpecs;

        public int Count;
    }

    [CreateAssetMenu(fileName = "fleet_roster", menuName = "GH/Fleet Roster")]
    public class FleetRosterData : ScriptableObject
    {
        public EFactions Faction;

        public int FleetID;

        public List<FleetShipTypesEntry> ShipTypeEntries;
    }
}