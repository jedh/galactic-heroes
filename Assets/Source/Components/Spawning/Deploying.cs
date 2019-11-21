using GH.Enums;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace GH.Components
{
    [Serializable]
    public struct Deploying : IComponentData
    {
        public int FleetID;
    }
}
