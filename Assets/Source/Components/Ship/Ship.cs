using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GH.Components
{
    [Serializable]
    public struct Ship : IComponentData
    {
        // Non-unique ship ID (ship type).
        public int ID;

        // Unique ship ID.
        public int InstanceID;
    }
}
