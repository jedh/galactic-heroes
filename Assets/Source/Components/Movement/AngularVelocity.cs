using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GH.Components
{
	[Serializable]
	public struct AngularVelocity : IComponentData
	{
		public float3 Axis;
        public float Velocity;
	}
}