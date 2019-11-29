using System;
using Unity.Entities;
using Unity.Mathematics;

namespace GH.Components
{
    [Serializable]
    public struct MovingTo : IComponentData
    {
        public float3 Position;
        public bool ShouldSTop;
    }
}