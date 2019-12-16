using GH.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
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

        public float ThrustTolerance;

        public float MaxSpeedToTurn;

        public bool DoesSwarm;

        public int SquadSize;

        public float3 HullArc;
        public float3 Hull;

        public float3 ShieldArc;
        public float3 Shield;

        public float HP;
    }
}