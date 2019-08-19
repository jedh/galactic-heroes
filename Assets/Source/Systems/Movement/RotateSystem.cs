using GH.Components;
using Unity.Entities;
using Unity.Transforms;

namespace GH.Systems
{
	public class RotateSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entities.ForEach((ref Translation translation, ref Rotation rotation, ref MoveSpeed moveSpeed) =>
			{
			});
		}
	}
}