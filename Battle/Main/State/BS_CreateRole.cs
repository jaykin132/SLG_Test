using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ET.Battle
{
	public class BS_CreateRole : BS_Base
	{
		public BS_CreateRole(BattleSystem bs) : base(bs)
		{

		}

		protected override void _OnEnter()
		{
			Debug.Log("[BS] RoleSystem Create");

			// 依據 RoleSetupTable 建立角色
			System.RoleSetupTable = "";
			RoleSystem.main.CreateRoles( () => SetStateToEnd() );
		}
	}
}