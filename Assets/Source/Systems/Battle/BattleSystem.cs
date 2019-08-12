using GH.Enums;
using GH.SystemGroups;
using Unity.Entities;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleSystemGroup))]
    public class BattleSystem : ComponentSystem
    {
        public EBattlePhases CurrentPhase = EBattlePhases.Unknown;

        //private SpawnBattleEntitiesSystem m_SpawnBattleEntitiesSystem;

        protected override void OnStartRunning()
        {
            //m_SpawnBattleEntitiesSystem = World.Active.GetExistingSystem<SpawnBattleEntitiesSystem>();
        }

        protected override void OnUpdate()
        {
            //if (CurrentPhase == EBattlePhases.Setup && m_SpawnBattleEntitiesSystem.IsFinished)
            //{
            //    CurrentPhase = EBattlePhases.Order;
            //    Debug.Log("Battle is setup, moving to order phase.");

            //    Entities.WithAll<CrewMember>().WithNone<CrewInput>().ForEach((Entity entity) =>
            //    {
            //        PostUpdateCommands.AddComponent(entity, default(CrewInput));
            //    });
            //}
        }
    }
}