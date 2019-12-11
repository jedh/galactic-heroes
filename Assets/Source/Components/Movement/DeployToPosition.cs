using System;
using Unity.Entities;
using Unity.Mathematics;

namespace GH.Components
{
	[Serializable]
	public struct DeployToPosition : IComponentData
	{
		public float3 Position;
		public bool ShouldStop;
	}
}