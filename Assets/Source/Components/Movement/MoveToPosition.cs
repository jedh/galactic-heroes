using System;
using Unity.Entities;
using Unity.Mathematics;

namespace GH.Components
{
    [Serializable]
    public struct MoveToPosition : IComponentData
    {
        public float3 Value;
    }
}