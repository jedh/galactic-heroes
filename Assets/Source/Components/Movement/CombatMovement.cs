using Unity.Entities;

namespace GH.Components
{
	public struct CombatMovement : IComponentData
	{
		public float MinRangeSq;

		public float MaxRangeSq;

		public float OptimalRangeSq;
	}
}