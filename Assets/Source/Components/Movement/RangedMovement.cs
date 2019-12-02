using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GH.Components
{
    public struct RangedMovement : IComponentData
    {
        public float MinRangeSq;

        public float MaxRangeSq;

        public float OptimalRangeSq;
    }
}
