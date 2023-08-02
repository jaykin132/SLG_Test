using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Battle
{
	public enum GridType
	{
		Map = 0,	// µØˆD¸ñ×Ó
		Move,		// ÒÆ„Ó¹ ‡ú
	}

	public enum SelType
	{
		None = 0,
		Select
	}

	public class GridDrawer : MonoBehaviour
	{
		public Vector2Int		Coordinate		= Vector2Int.zero;	
		public Renderer			Grid			= null;
		public Renderer			SelectOutline	= null;
		public List<Material>	GridMaterials	= new();
		public List<Material>	SelectMaterials = new();

		void Start() { }

		public void ChangeTo(GridType type)
		{
			switch (type)
			{
				case GridType.Map:
					Grid.material = GridMaterials[(int)GridType.Map];
					SelectOutline.material = SelectMaterials[(int)SelType.None];
					break;
				case GridType.Move:
					Grid.material = GridMaterials[(int)GridType.Move];
					SelectOutline.material = SelectMaterials[(int)SelType.None];
					break;
			}
		}

		public void Select()
		{
			SelectOutline.material = SelectMaterials[(int)SelType.Select];
		}

		public void UnSelect()
		{
			SelectOutline.material = SelectMaterials[(int)SelType.None];
		}
	}
}