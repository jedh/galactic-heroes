﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GH.Components
{
	[Serializable]
	public struct MovementStats : IComponentData
	{
		public float TopSpeed;

		public float Acceleration;

		public float Deceleration;

		public float RotationSpeed;
	}
}