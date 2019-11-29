using GH.SystemGroups;
using GH.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleLogicSystemGroup))]
	public class FindTargetSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entities.WithAll<FindTarget>().WithNone<Target>().ForEach((Entity entity, SharedFleetGrouping fleet, ref Translation translation, ref Rotation rotation) =>
			{
				Entity targetEntity = Entity.Null;
				var currentClosestDistanceSq = 0f;
				var trans = translation;
				Entities.ForEach((Entity otherEntity, SharedFleetGrouping otherFleet, ref Ship ship, ref Translation otherTranslation) =>
				{
					if (fleet.ID != otherFleet.ID)
					{
						var distanceSq = math.distancesq(otherTranslation.Value, trans.Value);
						if (targetEntity == Entity.Null || distanceSq < currentClosestDistanceSq)
						{
							currentClosestDistanceSq = distanceSq;
							targetEntity = otherEntity;
						}
					}
				});

				if (targetEntity != Entity.Null)
				{
					PostUpdateCommands.RemoveComponent(entity, typeof(FindTarget));
					PostUpdateCommands.AddComponent(entity, new Target() { TargetEntity = targetEntity });
				}
			});
		}
	}
}