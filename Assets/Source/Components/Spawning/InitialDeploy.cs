using System;
using Unity.Entities;

namespace GH.Components
{
    [Serializable]
    public struct InitialDeploy : IComponentData
    {
        public int FleetID;
    }
}
