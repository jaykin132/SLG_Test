using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ET.Battle
{
	public class MoveRange : DrawRange
	{
		protected int			Distance	= 1;
		protected Vector2Int	Center		= Vector2Int.zero;

		public MoveRange(Vector2Int pos, int distance)
		{
			Center		= pos;
			Distance	= distance;
		}

		public override void Draw(Action onDrawEnd = null)
		{
			async void m()
			{
				for (int d = 0; d <= Distance; ++d)
				{
					var range = GetRingRange(Center, d);
					foreach (var grid in range)
					{
						if (d == 0 || MapSystem.main.IsCanMoveTo(grid.Coordinate))
						{
							grid.ChangeTo(GridType.Move);
						}

						AddGridList(grid);
					}

					await Task.Delay(50);
				}

				onDrawEnd?.Invoke();
			}

			m();
		}
	}

	public abstract class DrawRange
	{
		protected List<GridDrawer> GridList = new List<GridDrawer>();

		public abstract void Draw(Action onDrawEnd = null);

		public bool InRange(int x, int y)
		{
			foreach (var grid in GridList)
			{
				if (grid.Coordinate.x == x && grid.Coordinate.y == y)
					return true;
			}

			return false;
		}

		public void Clear()
		{
			if (GridList.Count > 0)
			{
				foreach (var grid in GridList)
				{
					grid.ChangeTo(GridType.Map);
				}

				GridList.Clear();
			}
		}

		protected void AddGridList(GridDrawer gridDrawer)
		{
			GridList.Add(gridDrawer);
		}

        protected List<GridDrawer> GetRingRange(Vector2Int center, int distance, bool isFull = false)
        {
			var result  = new List<GridDrawer>();
			var gPoses  = new List<Vector2Int>();
            var xMix    = center.x - distance;
            var xMax    = center.x + distance;
            var yMix    = center.y - distance;
            var yMax    = center.y + distance;
			var md		= MapSystem.main.MapData;
			var XCount	= md.XCount;
			var YCount	= md.YCount;

			for (int x = xMix; x <= xMax; ++x)
            {
				for (int y = yMix; y <= yMax; ++y)
                {
                    if (x < 0 || x >= XCount || y < 0 || y >= YCount)
                        continue;

					var grid = md[x, y];
					if (grid != null)
                    {
						var check = Mathf.Abs(center.x - x) + Mathf.Abs(center.y - y);
						if ( isFull ? (check <= distance) : (check == distance) )
                        {
							gPoses.Add(new Vector2Int(x, y));
						}
					}
				}
			}

			foreach (var gPos in gPoses)
			{
				result.Add(MapDrawer.main.GetGridDrawer(gPos.x, gPos.y));
			}

			return result;
		}

		public static DrawRange Create<T>() where T : DrawRange, new()
		{
			return new T();
		}
	}
}