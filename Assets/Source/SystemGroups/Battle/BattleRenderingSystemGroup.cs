﻿using Unity.Entities;

namespace GH.SystemGroups
{
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [DisableAutoCreation]
    public class BattleRenderingSystemGroup : ComponentSystemGroup
	{

	}
}