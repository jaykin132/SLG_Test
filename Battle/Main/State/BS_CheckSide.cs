using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Battle
{
	public class BS_CheckSide : BS_Base
	{
		public BS_CheckSide(BattleSystem bs) : base(bs)
		{

		}

		protected override void _OnEnter()
		{
			var change = RoleSystem.main.IsSideActionDone();

			if (change)
			{
				RoleSystem.main.ChangeSide();
			}

			System.NestState = RoleSystem.main.CurrentSide switch
			{
				RsSide.Player		=> BattleSystem.BS_PlayerInput,
				RsSide.PlayerNpc	=> "",
				RsSide.Enemy		=> "",
				_					=> "",
			};

			SetStateToEnd();
		}
	}
}