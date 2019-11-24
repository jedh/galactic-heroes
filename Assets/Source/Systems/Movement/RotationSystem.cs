using GH.Components;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace GH.Systems
{
	public class RotationSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entities.ForEach((ref Rotation rotation, ref AngularVelocity angularVelocity) =>
			{
                if (angularVelocity.Velocity != 0f && (angularVelocity.Axis.x != 0f || angularVelocity.Axis.y != 0f || angularVelocity.Axis.z != 0f))
                {
                    rotation.Value = math.mul(rotation.Value, quaternion.AxisAngle(angularVelocity.Axis, angularVelocity.Velocity * Time.deltaTime));
                }
			});
		}
	}
}