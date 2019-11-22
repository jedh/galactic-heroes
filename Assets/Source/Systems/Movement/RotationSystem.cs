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
                rotation.Value = math.mul(rotation.Value, quaternion.RotateY(angularVelocity.Value * Time.deltaTime));  //assumes rotation about Y axis...
			});
		}
	}
}