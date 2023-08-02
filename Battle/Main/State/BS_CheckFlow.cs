using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Battle
{
	public class BS_CheckFlow : BS_Base
	{
		public BS_CheckFlow(BattleSystem battleSystem) : base(battleSystem)
		{
		}

		protected override void _OnEnter()
		{
			var flowToState = System.NestState;

			// 大部份會在此判斷下個狀態，但也可能由事件指定。
			if (flowToState == "")
			{
				var roleSetupTable = System.RoleSetupTable;

				if (roleSetupTable == "")
				{
					// 1，check is win or lose or continue
					// 2，RoundStart or RoundEnd
					// 3，RoleAction continue

					var isWin	= CheckIsWin();
					var isLose	= !isWin && CheckIsLose();

					if (!isWin && !isLose)
					{
						if (RoleSystem.main.IsAllRoleCanAction())
						{ // 所有角色未行動，開始新回合

							flowToState = BattleSystem.BS_RoundStart;
						}
						else if (RoleSystem.main.IsAllRoleActionDone())
						{ // 部份角色未行動，繼續行動

							flowToState = BattleSystem.BS_RoundEnd;
						}
						else
						{ // 所有角色行動完畢，結束回合

							flowToState = BattleSystem.BS_CheckSide;
						}
					}
					else
					{ // win or lose

						flowToState = isWin ? BattleSystem.BS_Victory : BattleSystem.BS_Lose;
					}
				}
				else
					flowToState = BattleSystem.BS_RoleCreate;

				System.NestState = flowToState;
			}

			SetStateToEnd();
		}

		protected bool CheckIsWin()
		{
			var ret = RoleSystem.main.IsEnemyAllDead();
			return ret;
		}

		protected bool CheckIsLose()
		{
			var ret = RoleSystem.main.IsPlayerAllDead();
			return ret;
		}
	}
}