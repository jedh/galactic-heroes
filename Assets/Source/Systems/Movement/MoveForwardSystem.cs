using Unity.Entities;
using Unity.Transforms;
using GH.Components;
using UnityEngine;
using Unity.Mathematics;

namespace GH.Systems
{
	public class MoveForwardSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
            Entities.ForEach((ref MoveSpeed speed, ref Translation translation, ref Rotation rotation) => 
            {
                translation.Value = translation.Value + (math.forward(rotation.Value) * (speed.Value * Time.deltaTime));
            });

		}
	}
}