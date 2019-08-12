using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH.Data
{
    [CreateAssetMenu(fileName = "ship_specs", menuName = "GH/Ship Specs")]
    public class ShipSpecData : ScriptableObject
    {
        public string ID;

        public float Speed;

        public float TurnSpeed;

        public float Hull;

        public float Shields;

        public float ShieldRechargeRate;

        public int CrewCount;
    }
}