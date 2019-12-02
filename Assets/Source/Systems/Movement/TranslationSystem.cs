using GH.Components;
using GH.SystemGroups;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleLogicSystemGroup))]
    public class TranslationSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new TranslationJob() { DeltaTime = Time.deltaTime }.Schedule(this, inputDeps);
        }

        //--------------------------
        // Jobs
        //--------------------------

        [BurstCompile]
        struct TranslationJob : IJobForEach<Translation, Velocity>
        {
            public float DeltaTime;

            public void Execute(ref Translation translation, [ReadOnly] ref Velocity velocity)
            {
                translation.Value = translation.Value + (velocity.Value * DeltaTime);
            }
		}
	}
}