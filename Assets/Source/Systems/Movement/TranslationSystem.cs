using Unity.Entities;
using Unity.Transforms;
using GH.Components;
using GH.SystemGroups;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class TranslationSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
            Entities.ForEach((ref Translation translation, ref Velocity velocity) => 
            {
                translation.Value = translation.Value + (velocity.Value * Time.deltaTime);
            });
		}
	}
}