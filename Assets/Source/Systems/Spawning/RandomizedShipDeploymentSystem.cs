using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleLogicSystemGroup))]
	public class RandomizedInitialDeploymentSystem : ComponentSystem
	{
		Unity.Mathematics.Random m_Random;

		protected override void OnCreate()
		{
			m_Random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
		}

		protected override void OnUpdate()
		{
			Entities.ForEach((Entity entity, ref InitialDeploy deploy, ref Translation translation) =>
			{
				float3 randomPosition = m_Random.NextFloat3(-10f, 10f);
				randomPosition.y = 0f;

				float zOffset = 6f;
				if (deploy.FleetID == 1)
				{
					zOffset = -zOffset;
				}

				randomPosition.z = (randomPosition.z * 0.1f) + zOffset;

				float3 startingPosition = randomPosition;
				startingPosition.z += zOffset;

				translation.Value = startingPosition;

				PostUpdateCommands.AddComponent<DeployToPosition>(entity);
				PostUpdateCommands.SetComponent(entity, new DeployToPosition() { Position = randomPosition, ShouldStop = true });

				PostUpdateCommands.RemoveComponent<InitialDeploy>(entity);
			});
		}
	}
}