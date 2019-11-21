using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleSetupSystemGroup))]
    public class RandomizedShipDeploymentSystem : ComponentSystem
    {
        Unity.Mathematics.Random m_Random;

        protected override void OnCreate()
        {
            m_Random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        }

        protected override void OnUpdate()
        {
            Entities.WithNone<Deploying>().ForEach((Entity entity, ref Deploy deploy, ref Translation translation) =>
            {
                float3 randomPosition = m_Random.NextFloat3(-5f, 5f);
                randomPosition.y = 0f;

                float zOffset = 5f;
                if (deploy.FleetID == 1)
                {
                    zOffset = -5f;
                }

                randomPosition.z = (randomPosition.z / 5f) + zOffset;

                float3 startingPosition = randomPosition;
                startingPosition.z += zOffset;

                translation.Value = startingPosition;

                EntityManager.RemoveComponent<Deploy>(entity);
                EntityManager.AddComponent<Deploying>(entity);
                EntityManager.AddComponentData(entity, new MovementTarget() { Value = randomPosition });
            });

            Entities.WithAll<Deploying>().WithNone<MovementTarget>().ForEach((Entity entity) =>
            {
                EntityManager.RemoveComponent<Deploying>(entity);

            });
        }
    }
}