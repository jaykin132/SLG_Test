using EzTool;
using UnityEngine;

namespace ET.Battle
{
	public class GridData
	{
		public MapData	Parent = null;

		public int		X				= 0;
		public int		Y				= 0;
		public Vector3	LocalPosition	= Vector3.zero;
		public bool		IsRoad			= false;

		public Vector2Int Coordinate	=> new(X, Y);

		public Vector2 Size				=> Parent.GridSize;
		public Vector3 LocalCenter		=> LocalPosition + new Vector3(Size.x / 2, 0, Size.y / 2);

		public Vector3 Center			=> Parent.Position + LocalCenter;
		public Vector3 Position			=> Parent.Position + LocalPosition;

		// store to binary
        public void Store(BinaryStore bs)
		{
			bs.SetInt(X);
			bs.SetInt(Y);
			bs.SetVector3(LocalPosition);
			bs.SetBool(IsRoad);
		}

		// restore from binary
		public void Restore(BinaryStore bs)
		{
			X				= bs.GetInt();
			Y				= bs.GetInt();
			LocalPosition	= bs.GetVector3();
			IsRoad			= bs.GetBool();
		}

		public bool Equal(GridData gd)
		{ // compare two grids with all£¬simply write

			var equal = gd != null;

			if (equal)
				equal = equal && X == gd.X;

			if (equal)
				equal = equal && Y == gd.Y;

			if (equal)
				equal = equal && LocalPosition == gd.LocalPosition;

			if (equal)
				equal = equal && IsRoad == gd.IsRoad;

			return equal;
		}
	}

	public class GridMap
	{
		public MapData		Parent = null;
		public GridData[,]	Grids = null;

		public GridData this[int x, int y]
		{
			get
			{
				if (x < 0 || x >= Grids.GetLength(0))
					return null;

				if (y < 0 || y >= Grids.GetLength(1))
					return null;

				return Grids[x, y];
			}
		}

		public GridMap(MapData md) { Parent = md; }

		public GridMap(MapData md, int xCount, int yCount, float gWidth, float gHeight, bool isRoad)
		{
			Grids = new GridData[xCount, yCount];

			for (int y = 0; y < yCount; ++y)
			{
				for (int x = 0; x < xCount; ++x)
				{
					Grids[x, y] = new GridData
					{
						Parent = md,
						X = x,
						Y = y,
						LocalPosition = new Vector3()
						{
							x = x * gWidth,
							y = 0f,
							z = y * gHeight
						},
						IsRoad = isRoad
					};
				}
			}
		}

		public void Store(BinaryStore bs) 
		{
			bs.SetInt(Grids.GetLength(0));
			bs.SetInt(Grids.GetLength(1));

			for (int y = 0; y < Grids.GetLength(1); ++y)
			{
				for (int x = 0; x < Grids.GetLength(0); ++x)
				{
					var grid = Grids[x, y];
					grid.Store(bs);
				}
			}
		}

		public void Restore(BinaryStore bs) 
		{
			int xCount = bs.GetInt();
			int yCount = bs.GetInt();

			Grids = new GridData[xCount, yCount];

			for (int y = 0; y < yCount; ++y)
			{
				for (int x = 0; x < xCount; ++x)
				{
					GridData grid = new()
					{
						Parent = Parent
					};

					grid.Restore(bs);
					Grids[x, y] = grid;
				}
			}
		}

		public bool Equal(GridMap gm)
		{
			var equal = gm != null;

			if (equal)
				equal = equal && Grids.GetLength(0) == gm.Grids.GetLength(0);

			if (equal)
				equal = equal && Grids.GetLength(1) == gm.Grids.GetLength(1);

			if (equal)
			{
				for (int y = 0; y < Grids.GetLength(1); ++y)
				{
					for (int x = 0; x < Grids.GetLength(0); ++x)
					{
						var grid = Grids[x, y];
						equal = equal && grid.Equal(gm.Grids[x, y]);

						if (!equal)
							break;
					}
				}
			}

			return equal;
		}
	}
}