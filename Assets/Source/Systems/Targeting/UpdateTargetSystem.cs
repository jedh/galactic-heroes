using GH.SystemGroups;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleLogicSystemGroup))]
	public class UpdateTargetSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
		}
	}
}