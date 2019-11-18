using GH.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

namespace GH.Systems
{
	public class UpdateShipViewSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entities.WithAll<Ship, View>().ForEach((ViewSync viewSync, ref Translation translation, ref Rotation rotation) =>
			{
				viewSync.transform.position = translation.Value;
				viewSync.transform.rotation = rotation.Value;
			});
		}
	}
}