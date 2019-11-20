using GH.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GH.Components
{
    [Serializable]
    public struct SpawnFleet : IComponentData
    {
        public int ShipID;

        public int ShipCount;

        public EFactions Faction;

        public int FleetID;

        public float TopSpeed;

        public float RotationSpeed;

        public float Acceleration;

        public float Deceleration;
    }
}