using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzTool;
using StateBase = EzTool.FSM.StateBase;

namespace ET.Battle
{
	public class BS_MapInit : BS_Base
	{
		public BS_MapInit(BattleSystem bs) : base(bs)
		{

		}

		protected override void _OnEnter()
		{
			Debug.Log("[BS] MapSystem Init");

			// 之後看怎麼讀表或事件指定

			System.LastState		= "";
			System.RoleSetupTable	= "Test";
			System.NestState		= "";

			RoleSystem.main.CurrentSide = RsSide.Player;

			SetStateToEnd();
		}
	}
}
