//using GH.Components;
//using GH.SystemGroups;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Transforms;
//using UnityEngine;

//namespace GH.Systems
//{
//	[UpdateInGroup(typeof(BattleLogicSystemGroup))]
//	public class DetermineMoveTargetSystem : ComponentSystem
//	{
//		[ReadOnly]
//		public ComponentDataFromEntity<Translation> TranslationData;

//		protected override void OnUpdate()
//		{
//			TranslationData = GetComponentDataFromEntity<Translation>(true);

//			Entities.WithNone<DeployToPosition>().ForEach((Entity e, ref Target target, ref Translation translation) =>
//			{
//				if (TranslationData.Exists(target.TargetEntity))
//				{
//					var position = TranslationData[target.TargetEntity];
//					PostUpdateCommands.AddComponent(e, new DeployToPosition() { Position = position.Value });
//				}
//			});

//			Entities.ForEach((Entity e, ref Target target, ref DeployToPosition deployToPosition, ref Translation translation, ref MovementStats movementStats) =>
//			{
//				if (TranslationData.Exists(target.TargetEntity))
//				{
//					var position = TranslationData[target.TargetEntity];
//					var shouldStop = !movementStats.DoesSwarm;
//					PostUpdateCommands.SetComponent(e, new DeployToPosition() { Position = position.Value, ShouldStop = shouldStop });
//				}
//			});
//		}
//	}
//}