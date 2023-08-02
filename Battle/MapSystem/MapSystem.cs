using EzTool;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ET.Battle
{
	public class MapSystem
	{
		public	string	SceneName	= "SampleScene";
		public 	MapData	MapData		= null;
		public  AStar	AStar		= null;

		protected DrawRange _MoveRange = null;

		public bool Load()
		{
			return Load(SceneName);
		}

		public bool Load(string sceneName)
		{
			SceneName = sceneName;
			MapData = new MapData();
			
			var ret = MapData.Load(sceneName);
			if (ret)
			{
				AStar = new AStar(MapData.XCount, MapData.YCount)
				{
					IsPassableMethod = _ => IsCanMoveTo(_)
				};
			}

			return ret;
		}

		public bool IsCanMoveTo(Vector2Int pos)
		{
			return IsCanMoveTo(pos.x, pos.y);
		}

		public bool IsCanMoveTo(int x, int y)
		{
			var grid	= MapData[x, y];
			var ret		= true; //grid?.IsRoad ?? false;

			if (ret)
				ret = _MoveRange?.InRange(x, y) ?? true;

			if (ret)
				ret = !RoleSystem.main.HaveBattleRole(x, y);
			
			return ret;
		}

		public List<GridData> FindPath(Vector2Int from, Vector2Int to, DrawRange moveRange)
		{
			return FindPath(from.x, from.y, to.x, to.y, moveRange);
		}

		public List<GridData> FindPath(int fx, int fy, int tx, int ty, DrawRange moveRange)
		{
			_MoveRange = moveRange;

			var nodes = AStar.FindPath(fx, fy, tx, ty);
			var ret = new List<GridData>();

			if (nodes.Count > 0)
			{
				ret = nodes.Select(_ => MapData[_.x, _.y]).ToList();
			}

			_MoveRange = null;
			return ret;
		}

		public void SetBattleRoleToGrid(BattleRole br)
		{
			SetActorToGrid(br.gameObject, br.Coordinate.x, br.Coordinate.y, br.Forward);	
		}

		public void SetActorToGrid(GameObject actor, Vector2Int pos)
		{
			SetActorToGrid(actor, pos.x, pos.y, Vector3.forward);
		}

		public void SetActorToGrid(GameObject actor, Vector2Int pos, Vector3 forward)
		{
			SetActorToGrid(actor, pos.x, pos.y, forward);
		}

		public void SetActorToGrid(GameObject actor, int x, int y)
		{
			SetActorToGrid(actor, x, y, Vector3.forward);
		}

		public void SetActorToGrid(GameObject actor, int x, int y, Vector3 forward)
		{
			if (actor != null)
			{
				actor.transform.position = GetGridCenter(x, y);
				actor.transform.forward = forward;
			}
		}

		public GridData ScreenPositionToGrid(Vector3 screenPosition)
		{
			var local = MapDrawer.main.ScreenTouchTransformPosition(screenPosition);
			return (local.x == -1 || local.z == -1) ? null : LocalPositionToGrid(local);
		}

		public GridData LocalPositionToGrid(Vector3 localPosition)
		{
			var x = (int)(localPosition.x / MapData.GridSize.x);
			var y = (int)(localPosition.z / MapData.GridSize.y);

			return MapData[x, y];
		}

		public GridData PositionToGrid(Vector3 worldPosition)
		{
			var local = MapDrawer.main.TransformPosition(worldPosition);
			var grid  = LocalPositionToGrid(local);

			return grid;
		}

		public GridData GetGrid(Vector2Int coord)
		{
			return MapData[coord.x, coord.y];
		}

		public Vector3 GetGridCenter(Vector2Int pos)
		{
			return GetGridCenter(pos.x, pos.y);
		}

		public Vector3 GetGridCenter(int x, int y)
		{
			return MapData[x, y]?.Center ?? Vector3.zero;
		}

		protected static MapSystem _main = null;
		public static MapSystem main
		{
			get
			{
				if (_main == null)
				{
					_main = new MapSystem();
				}
				return _main;
			}
		}
	}
}