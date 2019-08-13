using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GH.Components
{
    [Serializable]
    public struct Ship : IComponentData
    {
        public int ID;
    }
}
