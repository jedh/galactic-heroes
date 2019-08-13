﻿using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleSetupSystemGroup))]
    [UpdateAfter(typeof(SpawnBattleMapSystem))]
    public class SpawnBattleEntitiesSystem : ComponentSystem
    {
        private int m_SpawingEntityCount = 0;
        private int m_EntitySpawnedCount = 0;

        public bool IsFinished { get; private set; } = false;

        protected override void OnUpdate()
        {
            // Spawn ships.
            Entities.WithAll<SpawnShip>().WithNone<SpawnEntity, SpawnEntityState>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.AddComponent(entity, default(SpawnEntity));
                Debug.Log("spawn starting");
                m_SpawingEntityCount++;
            });

            // TODO: Query for SpawnEntityState which don't have corresponding SpawnEntity.
            Entities.WithAll<SpawnEntityState>().WithNone<SpawnEntity>().ForEach((Entity entity) =>
            {
                Debug.Log("spawning finished");
                m_EntitySpawnedCount++;
                PostUpdateCommands.RemoveComponent(entity, typeof(SpawnEntityState));
            });

            if (m_EntitySpawnedCount == m_SpawingEntityCount)
            {
                Debug.Log("All entities have been spawned.");
                IsFinished = true;
            }
        }
    }
}