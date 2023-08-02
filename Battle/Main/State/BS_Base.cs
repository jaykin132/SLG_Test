using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzTool;
using StateBase = EzTool.FSM.StateBase;

namespace ET.Battle
{
	public class BS_Base : StateBase
	{
		protected BattleSystem System = null;

		public BS_Base(BattleSystem bs)
		{
			System = bs;
		}

		protected override void			OnEnter()					{ _OnEnter(); }
		protected override void			OnUpdate(float dt)			{ _OnUpdate(dt); }
		protected override StateBase	OnNextState()				{ return _OnNextState(); }
		protected override void			OnExit()					{ _OnExit(); }
		protected override void			OnFrom(StateBase lastState) { _OnFrom(lastState); }
		protected override void			OnDispose()					{ _OnDispose(); }

		protected virtual void		_OnEnter()						{ }
		protected virtual void		_OnUpdate(float dt)				{ }
		protected virtual StateBase	_OnNextState()					{ return DefaultNext; }
		protected virtual void		_OnExit()						{ }
		protected virtual void		_OnFrom(StateBase lastState)	{ }
		protected virtual void		_OnDispose()					{ }
	}
}