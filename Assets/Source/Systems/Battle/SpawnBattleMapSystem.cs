using GH.SystemGroups;
using Unity.Entities;

namespace GH.Systems
{
    [UpdateInGroup(typeof(BattleSetupSystemGroup))]
    public class SpawnBattleMapSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {

        }
    }
}