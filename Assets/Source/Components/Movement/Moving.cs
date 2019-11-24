using System;
using Unity.Entities;
using Unity.Mathematics;

namespace GH.Components
{
    [Serializable]
    public struct Moving : IComponentData
    {
        public float3 Value;
    }
}