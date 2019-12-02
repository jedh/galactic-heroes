using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GH.Components
{
    public struct WeaponStats : IComponentData
    {
        public float MinRange;

        public float MaxRange;

        public float OptimalRange;

        public float Damage;
    }
}