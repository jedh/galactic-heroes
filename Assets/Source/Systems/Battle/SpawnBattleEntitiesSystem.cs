using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using UnityEngine;

namespace GH.Systems
{
	[UpdateInGroup(typeof(BattleSetupSystemGroup))]
	[UpdateAfter(typeof(SpawnBattleMapSystem))]
	public class SpawnBattleEntitiesSystem : ComponentSystem
	{
		private int m_SpawningEntityCount = 0;
		private int m_EntitySpawnedCount = 0;

		public bool IsFinished { get; private set; } = false;

		protected override void OnUpdate()
		{
            // Spawn ships.
            Entities.WithAll<SpawnShip>().WithNone<SpawnEntity, SpawnEntityState>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.AddComponent(entity, default(SpawnEntity));
                Debug.Log("spawn starting");
                m_SpawningEntityCount++;
            });

            Entities.WithAll<SpawnFleet>().WithNone<SpawnEntity, SpawnEntityState>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.AddComponent(entity, default(SpawnEntity));
                Debug.Log("spawn fleet starting");
                m_SpawningEntityCount++;
            });

            // TODO: Query for SpawnEntityState which don't have corresponding SpawnEntity.
            Entities.WithAll<SpawnEntityState>().WithNone<SpawnEntity>().ForEach((Entity entity) =>
			{
				Debug.Log("spawning finished");
				m_EntitySpawnedCount++;
				PostUpdateCommands.RemoveComponent(entity, typeof(SpawnEntityState));
			});

			if (m_EntitySpawnedCount == m_SpawningEntityCount)
			{
				Debug.Log("All entities have been spawned.");
				// TODO: Move to a global state object.
				IsFinished = true;
			}
		}
	}
}