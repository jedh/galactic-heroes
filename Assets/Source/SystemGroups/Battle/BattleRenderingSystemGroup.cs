using Unity.Entities;

namespace GH.SystemGroups
{
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	[DisableAutoCreation]
	public class BattleRenderingSystemGroup : ComponentSystemGroup
	{

	}
}