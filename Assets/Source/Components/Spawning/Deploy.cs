using GH.Enums;
using System;
using Unity.Entities;

namespace GH.Components
{
    [Serializable]
    public struct Deploy : IComponentData
    {
        public int FleetID;
    }
}
