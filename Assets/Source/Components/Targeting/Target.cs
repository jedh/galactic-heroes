﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GH.Components
{
	public struct Target : IComponentData
	{
		public Entity TargetEntity;
	}
}