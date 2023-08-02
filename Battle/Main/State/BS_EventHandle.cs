using EzTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Battle
{
	public class BS_EventHandle : BS_Base
	{
		public BS_EventHandle(BattleSystem bs) : base(bs)
		{

		}

		protected override void _OnEnter()
		{
			Debug.Log("[BS] EventHandle");

			if (LastState.Name == BattleSystem.BS_MapCreate)
			{
				System.NestState = BattleSystem.BS_RoleCreate;
			}
			else
			{
				System.NestState = "";
			}

			SetStateToEnd();
		}
	}
}
