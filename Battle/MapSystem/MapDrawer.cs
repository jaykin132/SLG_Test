using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using EzTool.Extensions;
using EzTool;
using System;

namespace ET.Battle
{
	public class MapDrawer : MonoBehaviour
	{
		public MapData Data = null;

		protected GameObject		GridContainer	= null;
		protected GridDrawer[,]		Grids			= null;

		private void Awake()
		{
			GridContainer = CreateGridContainer();
		}

		#region Public
		public AutoRun CreateMapAutoRun(MapData data)
		{
			Data	= data;
			Grids	= new GridDrawer[Data.XCount, Data.YCount];

			var x = 0;
			var y = 0;

			return AutoRun.Create()

					.LoopForver( _ => y >= Data.YCount )		

						.Do( _ => x = 0 )
						.LoopForver( _ => x >= Data.XCount )
							.Wait( _ => CreateGridAutoRun( _ =>
							{
								var drawer = _;

								var grid = Data[x, y];
								if (grid != null)
								{
									var go = drawer.gameObject;

									go.name = $"Grid_{x}_{y}";
									go.transform.SetParent(GridContainer.transform);
									go.transform.localPosition = grid.LocalCenter + new Vector3(0f, 0.01f, 0f);
									go.transform.localEulerAngles = Vector3.zero;
									go.transform.localScale = Vector3.one;

									drawer.Coordinate.x = x;
									drawer.Coordinate.y = y;
									Grids[x, y] = drawer;
								}
								else
								{
									Debug.LogError($"[MapDrawer] GridData is null at {x}, {y}");
									Grids[x, y] = null;
								}
							}))
							.Do( _ => ++x)
						.LoopEnd()

						.DelayOneFrame()
						.Do( _ => ++y )	
					.LoopEnd();
		}

		public async Task CreateMapAsync(MapData data)
		{
			Data	= data;
			Grids	= new GridDrawer[Data.XCount, Data.YCount];

			for (int y = 0; y < Data.YCount; y++)
			{
				for (int x = 0; x < Data.XCount; x++)
				{
					var grid = Data[x, y];
					if (grid != null)
					{
						var drawer	= await CreateGridAsync();
						var go		= drawer.gameObject;

						go.name = $"Grid_{x}_{y}";
						go.transform.SetParent(GridContainer.transform);
						go.transform.localPosition = grid.LocalCenter + new Vector3(0f, 0.01f, 0f);
						go.transform.localEulerAngles = Vector3.zero;
						go.transform.localScale = Vector3.one;

						drawer.Coordinate.x = x;
						drawer.Coordinate.y = y;
						Grids[x, y] = drawer;
					}
					else
					{
						Debug.LogError($"[MapDrawer] GridData is null at {x}, {y}");
						Grids[x, y] = null;
					}
				}

				await Task.Yield();
			}

			CreateTouchPlane();
		}

		public Vector3 ScreenTouchTransformPosition(Vector3 screenPos)
		{
			var ray = Camera.main.ScreenPointToRay(screenPos);
			if (Physics.Raycast(ray, out RaycastHit hit, 1000f, 1 << LayerMask.NameToLayer("GridLayer")))
			{
				var point = transform.TransformPoint(hit.point);
				return point;
			}

			return new Vector3(-1, -1, -1);
		}

		public Vector3 TransformPosition(Vector3 worldPosition)
		{
			return transform.TransformPoint(worldPosition);
		}

		public GridDrawer GetGridDrawer(GridData grid)
		{
			if (grid != null)
			{
				return GetGridDrawer(grid.X, grid.Y);
			}

			return null;
		}

		public GridDrawer GetGridDrawer(int x, int y)
		{
			if (x < 0 || x >= Data.XCount || y < 0 || y >= Data.YCount)
			{
				Debug.LogError($"[MapDrawer] GetGridDrawer out of range {x}, {y}");
				return null;
			}

			return Grids[x, y];
		}
		#endregion

		#region Protected / Private
		protected AutoRun CreateGridAutoRun(Action<GridDrawer> onEnd)
		{
			var opr = Addressables.LoadAssetAsync<GameObject>("Battle/Grid/Grid.prefab");
			return AutoRun.Create()
					.Where( _ => opr.IsDone )
					.Do( _ =>
					{
						if (opr.Result != null)
						{
							var go		= Instantiate(opr.Result);
							var drawer	= go.GetComponent<GridDrawer>();
							onEnd?.Invoke(drawer);
						}
					});

		}

		protected async Task<GridDrawer> CreateGridAsync()
		{
			var opr = Addressables.LoadAssetAsync<GameObject>("Battle/Grid/Grid.prefab");
			await opr.Task;

			GridDrawer drawer = null;
			if (opr.Result != null)
			{
				var go = Instantiate(opr.Result);
				drawer = go.GetComponent<GridDrawer>();
			}

			return drawer;
		}

		protected void CreateTouchPlane()
		{
			var go = GameObject.CreatePrimitive(PrimitiveType.Quad);

			go.name = "TouchPlane";
			go.transform.SetParent(GridContainer.transform);
			go.transform.localPosition = new Vector3(0f, 0.01f, 0f);
			go.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
			go.transform.localScale = new Vector3(Data.Size.x, Data.Size.y, 1f);

			// set GridLayer
			go.layer = LayerMask.NameToLayer("GridLayer");

			// Hide Plane
			var mr = go.GetComponent<MeshRenderer>();
			if (mr != null)
			{
				mr.enabled = false;
			}

			// localPosition set to container center
			go.transform.localPosition = new Vector3(Data.Size.x * 0.5f, 0.01f, Data.Size.y * 0.5f);
		}

		protected GameObject CreateGridContainer()
		{
			var go = new GameObject("GridContainer");
			go.transform.SetParent(transform);
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;

			return go;
		}
		#endregion


		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		protected static MapDrawer _main = null;
		public static MapDrawer main
		{
			get
			{
				if (_main == null)
				{
					var go = new GameObject("MapDrawer");
					_main = go.AddComponent<MapDrawer>();

					go.transform.localPosition	= Vector3.zero;
					go.transform.localRotation	= Quaternion.identity;
					go.transform.localScale		= Vector3.one;
				}

				return _main;
			}
		}
	}
}