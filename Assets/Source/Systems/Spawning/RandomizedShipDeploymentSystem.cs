using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleLogicSystemGroup))]
	public class RandomizedInitialDeploymentSystem : ComponentSystem
	{
		Unity.Mathematics.Random m_Random;

		const float k_Width = 20f;
		const float k_Height = 10f;

		protected override void OnCreate()
		{
			m_Random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
		}

		protected override void OnUpdate()
		{
			Entities.ForEach((Entity entity, ref InitialDeploy deploy, ref Translation translation, ref Rotation rotation) =>
			{
				float3 randomPosition = new float3(m_Random.NextFloat(-k_Width, k_Width), 0f, m_Random.NextFloat(-k_Height, k_Height));

				float zOffset = 6f;
				if (deploy.FleetID == 1)
				{
					zOffset = -zOffset;
				}
				else
				{
					rotation.Value = math.mul(rotation.Value, quaternion.AxisAngle(math.up(), math.radians(180f)));
				}

				randomPosition.z = (randomPosition.z * 0.1f) + zOffset;

				float3 startingPosition = randomPosition;
				startingPosition.z += zOffset;

				translation.Value = startingPosition;

				PostUpdateCommands.AddComponent(entity, new DeployToPosition() { Position = randomPosition, ShouldStop = true });

				PostUpdateCommands.RemoveComponent<InitialDeploy>(entity);
			});
		}
	}
}