using GH.Components;
using GH.SystemGroups;
using GH.Systems;
using Unity.Entities;
using UnityEngine;

namespace GH.Scripts
{
    public class BattleBootstrap : MonoBehaviour
    {
        BattleSystemGroup m_BattleSystemGroup;
        BattleInputSystemGroup m_BattleInputSystemGroup;
        BattleLogicSystemGroup m_BattleLogicSystemGroup;
        BattleRenderingSystemGroup m_BattleRenderingSystemGroup;
        BattleSetupSystemGroup m_BattleSetupSystemGroup;
        BattleSystem m_BattleSystem;

        // Start is called before the first frame update
        void Start()
        {
            var entityManager = World.Active.EntityManager;
            var battleEntity = entityManager.CreateEntity();
            entityManager.SetName(battleEntity, "Battle Entity");
            entityManager.AddSharedComponentData(battleEntity, default(SharedBattleLevel));
            entityManager.AddComponentData(battleEntity, default(BattleLevelSetup));

            // The top level component group for all battle systems.
            m_BattleSystemGroup = World.Active.GetOrCreateSystem<BattleSystemGroup>();

            // Battle component group sub systems.
            m_BattleInputSystemGroup = World.Active.GetOrCreateSystem<BattleInputSystemGroup>();
            m_BattleLogicSystemGroup = World.Active.GetOrCreateSystem<BattleLogicSystemGroup>();
            m_BattleRenderingSystemGroup = World.Active.GetOrCreateSystem<BattleRenderingSystemGroup>();
            m_BattleSetupSystemGroup = World.Active.GetOrCreateSystem<BattleSetupSystemGroup>();

            // BattleSystem should be one of the only systems that lives directly under the BattleSystemGroup.
            m_BattleSystem = World.Active.GetOrCreateSystem<BattleSystem>();
            m_BattleSystem.CurrentPhase = Enums.EBattlePhases.Setup;
        }

        private void Update()
        {
            // Always update the top level group first.
            m_BattleSystemGroup.Update();

            m_BattleRenderingSystemGroup.Update();

            if (m_BattleSystem.CurrentPhase == Enums.EBattlePhases.Setup)
            {
                m_BattleSetupSystemGroup.Update();
                return;
            }

            m_BattleInputSystemGroup.Update();

            if (m_BattleSystem.CurrentPhase != Enums.EBattlePhases.Paused)
            {
                m_BattleLogicSystemGroup.Update();
            }
        }
    }
}
