using GH.Components;
using GH.Data;
using GH.SystemGroups;
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
	[UpdateInGroup(typeof(BattleRenderingSystemGroup))]
	public class AddShipViewSystem : ComponentSystem
	{
		Transform m_ViewContainer;
		AssetManager m_AssetManager;

		protected override void OnCreate()
		{
			m_ViewContainer = new GameObject("GameViews").transform;
		}

		protected override void OnStartRunning()
		{
			if (m_AssetManager == null)
			{
				m_AssetManager = GameObject.Find("AssetManager").GetComponent<AssetManager>();
			}
		}

		protected override void OnUpdate()
		{
			Entities.WithAll<View>().WithNone<ViewState>().ForEach((Entity entity, ref Ship ship) =>
			{
				ShipViewData shipViewData;
				if (m_AssetManager.ShipViewDataMap.TryGetValue(ship.ID, out shipViewData))
				{
					var go = Object.Instantiate(shipViewData.ViewPrefab, m_ViewContainer, false);
					go.name = $"Ship{ship.InstanceID}";
					var viewSync = go.AddComponent<ViewSync>();
					PostUpdateCommands.AddComponent(entity, default(ViewState));
					EntityManager.AddComponentObject(entity, viewSync);
				}
			});
		}
	}
}