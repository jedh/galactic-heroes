using GH.Components;
using GH.Data;
using GH.SystemGroups;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
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
        Dictionary<int, RenderMesh> m_ShipRenderMeshMap;

		protected override void OnCreate()
		{
            //m_ViewContainer = new GameObject("GameViews").transform;
            m_ShipRenderMeshMap = new Dictionary<int, RenderMesh>();
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
                    RenderMesh renderMesh;
                    if (!m_ShipRenderMeshMap.TryGetValue(ship.ID, out renderMesh))
                    {
                        renderMesh = new RenderMesh()
                        {
                            castShadows = UnityEngine.Rendering.ShadowCastingMode.Off,
                            layer = shipViewData.Mesh.sortingLayerID,
                            material = shipViewData.Mesh.sharedMaterial,
                            mesh = shipViewData.Filter.sharedMesh,
                            receiveShadows = false,
                            subMesh = shipViewData.Mesh.subMeshStartIndex
                        };
                    }

                    PostUpdateCommands.AddSharedComponent(entity, renderMesh);

                    //var go = Object.Instantiate(shipViewData.ViewPrefab, m_ViewContainer, false);
					//go.name = $"Ship{ship.InstanceID}";
					//var viewSync = go.AddComponent<ViewSync>();
					//PostUpdateCommands.AddComponent(entity, default(ViewState));
					//EntityManager.AddComponentObject(entity, viewSync);
				}
			});
		}
	}
}