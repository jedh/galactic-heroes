using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Components
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    [Serializable]
    public class SpawnShipAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var spawnShip = new SpawnShip()
            {
               Position = transform.position,
               Rotation = transform.rotation
            };

            dstManager.AddComponentData(entity, spawnShip);
        }
    }
}
