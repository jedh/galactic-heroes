using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Components
{
    [Serializable]
    public class SpawnShipAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
          
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            
        }
    }
}
