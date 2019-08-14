using GH.Enums;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GH.Components
{
    [Serializable]
    public struct SharedFaction : ISharedComponentData
    {
        public EFactions Faction;
    }
}