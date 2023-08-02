using EzTool;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ET.Battle
{
	public class MapData
	{
		public bool IsEditor = false;

		public int				XCount		= 9;
		public int				YCount		= 9;
		public Vector2			Size		= Vector2.zero;
		public Vector2			GridSize	= Vector2.zero;
		public Vector3			Anchor		= Vector3.zero;
		public int				Layers		= 1;
		public List<GridMap>	GridMaps	= new();

		public Vector3 Position //	= Vector3.zero; // World Position
		{
			get
			{
				if (!IsEditor)
				{
					return MapDrawer.main.transform.position;
				}

				return MapViewer.main.transform.position;
			}
			set { }
		}

		public GridData this[int x, int y]
		{
			get => this[0, x, y];
		}

		public GridData this[int level, int x, int y]
		{
			get => GridMaps[level][x, y];
		}

		public bool TryGet(int x, int y, out GridData gData)
		{
			return TryGet(0, x, y, out gData);
		}

		public bool TryGet(int level, int x, int y, out GridData gData)
		{
			var find = 0 <= level && level <= Layers;

			gData = null;

			if (find)
			{
				find = 0 <= x && x < XCount;
				find = find && 0 <= y && y < YCount;

				if (find)
				{
					gData = GridMaps[level][x, y];
				}
			}

			return find;
		}

		public GridData GetGridData(int x, int y)
		{
			return GetGridData(0, x, y);
		}

		public GridData GetGridData(int level, int x, int y)
		{ // get grid safely

			if (level < 0 || level >= Layers)
			{
				return null;
			}

			if (x < 0 || x >= XCount)
			{
				return null;
			}

			if (y < 0 || y >= YCount)
			{
				return null;
			}

			return GridMaps[level][x, y];
		}

		public void Save(string sceneName)
		{
			var bs = new BinaryStore();

			bs.SetInt(XCount);
			bs.SetInt(YCount);
			bs.SetVector2(Size);
			bs.SetVector2(GridSize);
			bs.SetVector3(Position);
			bs.SetVector3(Anchor);
			bs.SetInt(Layers);

			bs.SetInt(GridMaps.Count);
			for (int i = 0; i < GridMaps.Count; ++i)
			{
				GridMaps[i].Store(bs);
			}

			var file = sceneName + "_grids.bin";
			var path = Application.streamingAssetsPath + "/GameData/" + file.ToLower();

			bs.Save(path);
			bs.Dispose();
		}

		public bool Load(string sceneName)
		{
			var file = sceneName + "_grids.bin";
			var path = Application.streamingAssetsPath + "/GameData/" + file.ToLower();

			if (File.Exists(path))
			{
				var bs = new BinaryStore();
				bs.Load(path);

				XCount = bs.GetInt();
				YCount = bs.GetInt();
				Size = bs.GetVector2();
				GridSize = bs.GetVector2();
				Position = bs.GetVector3();
				Anchor = bs.GetVector3();
				Layers = bs.GetInt();

				int count = bs.GetInt();
				for (int i = 0; i < count; ++i)
				{
					GridMap gridMap = new(this);

					gridMap.Restore(bs);
					GridMaps.Add(gridMap);
				}

				bs.Dispose();
				return true;
			}

			return false;
		}

		public bool Eqaul(MapData md)
		{
			var equal = md != null;

			if (equal)
				equal = equal && XCount == md.XCount;

			if (equal)
				equal = equal && YCount == md.YCount;

			if (equal)
				equal = equal && Size == md.Size;

			if (equal)
				equal = equal && GridSize == md.GridSize;

			if (equal)
				equal = equal && Layers == md.Layers;

			if (equal)
				equal = equal && GridMaps.Count == md.GridMaps.Count;

			if (equal)
			{
				for (int i = 0; i < GridMaps.Count; ++i)
				{
					equal = equal && GridMaps[i].Equal(md.GridMaps[i]);
					if (!equal)
						break;
				}
			}

			return equal;
		}

		public static MapData Create(int xCount, int yCount, float gWidth, float gHeight, bool isRoad)
		{
			return Create(1, xCount, yCount, gWidth, gHeight, isRoad);
		}

		public static MapData Create(int layers, int xCount, int yCount, float gWidth, float gHeight, bool isRoad)
		{
			MapData mapData = new()
			{
				Layers	= layers,
				XCount	= xCount,
				YCount	= yCount,
				Size	= new Vector2(xCount * gWidth, yCount * gHeight),
				GridSize= new Vector2(gWidth, gHeight)
			};
			
			for (int i = 0; i < layers; ++i)
			{
				mapData.GridMaps.Add(new GridMap(mapData, xCount, yCount, gWidth, gHeight, isRoad));
			}
			
			return mapData;
		}
	}
}