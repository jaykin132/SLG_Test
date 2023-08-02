using EzTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EzTool.FSM;

namespace ET.Battle
{
	public class PI_Base : FSM.StateBase
	{
		protected BS_PlayerInput Handler = null;

		public PI_Base(BS_PlayerInput handler)
		{
			Handler = handler;
		}

		protected override void			OnEnter()					{ _OnEnter(); }
		protected override StateBase	OnNextState()				{ return _OnNextState(); }
		protected override void			OnExit()					{ _OnExit(); }
		protected override void			OnDispose()					{ _OnDispose(); }

		protected override void OnUpdate(float dt)		
		{
			if (Input.GetMouseButtonDown(0))
			{
				var grid = MapSystem.main.ScreenPositionToGrid(Input.mousePosition);
				if (grid != null)
				{
					OnGridTouched(grid);
				}
			}

			_OnUpdate(dt); 
		}

		protected Vector2Int CheckGridSelected(Vector3 mPos)
		{
			var gridPos = Vector2Int.zero;

			return gridPos;
		}

		#region State Virtual Method
		protected virtual void		_OnEnter()						{ }
		protected virtual void		_OnUpdate(float dt)				{ }
		protected virtual StateBase _OnNextState()					{ return DefaultNext; }
		protected virtual void		_OnExit()						{ }
		protected virtual void		_OnFrom(StateBase lastState)	{ }
		protected virtual void		_OnDispose()					{ }
		#endregion

		#region Input Virtual Method
		protected virtual void OnGridTouched(GridData grid) { }
		#endregion
	}
}