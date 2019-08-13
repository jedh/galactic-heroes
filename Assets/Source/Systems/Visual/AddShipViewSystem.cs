using GH.Components;
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
			Debug.Log("Adding views...");
			Entities.WithAll<View>().WithNone<ViewState>().ForEach((Entity entity, ref Ship ship) =>
			{
				var go = Object.Instantiate(m_AssetManager.ShipPrefab);
				go.name = $"Ship{ship.ID}";
				go.transform.SetParent(m_ViewContainer, false);

				var viewSync = go.AddComponent<ViewSync>();
				//viewSync.ViewGO = go;
				PostUpdateCommands.AddComponent(entity, default(ViewState));
			});
		}
	}
}