using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace ET.Battle
{
	public class PI_RoleAction : PI_Base
	{
		public enum eAction
		{
			Selecting,
			Selected,
			MoveTo,
			DrawMoveRange,
			Attack,
			Wait
		}

		protected eAction			Action		= eAction.Selecting;
		protected BattleRole		SelectBR	= null;
		protected GridDrawer		SelectGD	= null;
		protected DrawRange			MoveRange	= null;
		protected bool				Waiting		= false;

		protected GridData CurrentGD = null;

		public PI_RoleAction(BS_PlayerInput pi) : base(pi)
		{

		}

		protected override void _OnEnter()
		{
			Action = eAction.Selecting;
		}

		protected override void _OnUpdate(float dt)
		{
			if (Waiting) return;

			switch (Action)
			{
				case eAction.Selecting:
					{
						DoSelecting();
						break;
					}
				case eAction.Selected:
					{
						DoSelected();
						break;
					}
				case eAction.DrawMoveRange:
					{
						DoDrawMoveRange();
						break;
					}
				case eAction.MoveTo:
					{
						DoMoveTo();
						break;
					}
				case eAction.Attack:
					{
						DoAttackTo();
						break;
					}
			}
		}

		protected override void OnGridTouched(GridData grid)
		{
			if (Waiting) return;
			CurrentGD = grid;
		}

		protected void DoSelecting()
		{
			if (CurrentGD != null)
			{
				if (SelectGD != null)
				{
					SelectGD.UnSelect();
					SelectGD = null;
				}

				SelectBR = RoleSystem.main.SelectBattleRole(CurrentGD);
				if (SelectBR != null)
				{
					Action = eAction.DrawMoveRange;
				}
				else
				{
					SelectGD = MapDrawer.main.GetGridDrawer(CurrentGD);
					SelectGD.Select();

					CurrentGD = null;
				}
			}
		}

		protected void DoSelected()
		{
			if (Input.GetMouseButtonUp(1))
			{ // 右鍵取消選擇

				ResetSelectBR();
				Action = eAction.Selecting;
			}
			else if (CurrentGD != null)
			{
				var grid	= CurrentGD;
				var br		= RoleSystem.main.SelectBattleRole(grid);

				// 能否攻擊
				if (IsCanAttack(SelectBR, br))
				{
					Action = eAction.Attack;
				}
				else
				{
					if (br != null && br != SelectBR)
					{ // 切到其它角色

						ResetSelectBR();
						Action = eAction.Selecting;
					}
					else if (MoveRange.InRange(grid.Coordinate.x, grid.Coordinate.y))
					{ // 在角色移動範圍內

						if (br == null)
						{ // 移動到目的

							Action = eAction.MoveTo;
						}
						else
						{ // 點到自己不動作

							CurrentGD = null;
						}
					}
					else
					{ // 取消選擇
						ResetSelectBR();

						CurrentGD	= null;
						Action		= eAction.Selecting;
					}
				}
			}
		}

		protected void DoDrawMoveRange()
		{
			Waiting = true;

			MoveRange = new MoveRange(CurrentGD.Coordinate, 3);
			MoveRange.Draw(() =>
			{
				Waiting = false;
				Action = eAction.Selected;
			});

			CurrentGD = null;
		}

		protected void DoMoveTo()
		{
			var nodes = MapSystem.main.FindPath(SelectBR.Coordinate, CurrentGD.Coordinate, MoveRange);
			if (nodes.Count > 0)
			{
				Waiting = true;
				SelectBR.PlayMoveTo(nodes, () =>
				{
					Waiting = false;
					Action  = eAction.Selected;
				});
			}
			else
				Action = eAction.Selected;

			CurrentGD = null;;
		}

		protected void DoAttackTo()
		{
			Waiting = true;

			var target = RoleSystem.main.SelectBattleRole(CurrentGD);
			SelectBR.PlayAttackTo(target, () =>
			{
				Waiting = false;
				Action	= eAction.Selecting;
			});

			ResetSelectBR();
			CurrentGD = null;
		}

		protected void ResetSelectGD()
		{
			if (SelectGD != null)
			{
				SelectGD.UnSelect();
				SelectGD = null;
			}
		}

		protected void ResetSelectBR()
		{
			MoveRange.Clear();
			MoveRange = null;
			SelectBR = null;
		}

		protected bool IsCanAttack(BattleRole castor, BattleRole target)
		{
			if (castor != null && target != null)
			{
				var cPos = castor.Coordinate;
				var tPos = target.Coordinate;
				var offset = (tPos - cPos).magnitude;

				return offset == 1;
			}

			return false;
		}
	}
}