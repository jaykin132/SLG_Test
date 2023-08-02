using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzTool;
using static EzTool.FSM;
using System.Threading.Tasks;

namespace ET.Battle
{
	/***
	 * @brief �ˠ�Bֻ̎���Єӽ�ɫ��������̎��һЩȫ���ԵĽ��������
	 ***/
	public class BS_PlayerInput : BS_Base
	{
		public enum eAction
		{
			Selecting,
			Selected,
			MoveTo,
			DrawMoveRange,
			Attack,
			Waiting
		}

		protected bool									LockInput 			= false;
		protected BattleRole							ActionRole	= null;
		protected GridDrawer							SelectGrid			= null;
		protected DrawRange								MoveRange			= null;
		protected GridData								CurrentGrid			= null;
		protected System.Action							CurrentAction		= null;

		public BS_PlayerInput(BattleSystem bs) : base(bs)
		{
		}

		protected override void _OnEnter()
		{
			CurrentAction = DoSelecting;
		}

		protected override void _OnUpdate(float dt)
		{
			CurrentAction?.Invoke();

			if (Input.GetMouseButtonDown(0))
			{
				var grid = MapSystem.main.ScreenPositionToGrid(Input.mousePosition);
				if (grid != null)
				{
					CurrentGrid = grid;
				}
			}
		}

		protected override void _OnExit()
		{
		}

		#region Protected
		protected void DoWaiting() { }

		protected void DoSelecting()
		{
			if (CurrentGrid != null)
			{
				if (SelectGrid != null)
				{
					SelectGrid.UnSelect();
					SelectGrid = null;
				}

				ActionRole = RoleSystem.main.SelectBattleRole(CurrentGrid);
				if (ActionRole != null)
				{
					CurrentAction = DoDrawMoveRange;
				}
				else
				{
					SelectGrid = MapDrawer.main.GetGridDrawer(CurrentGrid);
					SelectGrid.Select();

					CurrentGrid = null;
				}
			}
		}

		protected void DoSelected()
		{
			if (Input.GetMouseButtonUp(1))
			{ // ���Iȡ���x��

				ResetSelectBR();
				CurrentAction = DoSelecting;
			}
			else if (CurrentGrid != null)
			{
				var grid	= CurrentGrid;
				var br		= RoleSystem.main.SelectBattleRole(grid);

				// �ܷ񹥓�
				if (IsCanAttack(ActionRole, br))
				{
					CurrentAction = DoAttackTo;
				}
				else
				{
					if (br != null && br != ActionRole)
					{ // �е�������ɫ

						ResetSelectBR();
						CurrentAction = DoSelecting;
					}
					else if (MoveRange.InRange(grid.Coordinate.x, grid.Coordinate.y))
					{ // �ڽ�ɫ�Ƅӹ�����

						if (br == null)
						{ // �Ƅӵ�Ŀ��

							CurrentAction = DoMoveTo;
						}
						else
						{ // �c���Լ�������

							CurrentGrid = null;
						}
					}
					else
					{ // ȡ���x��
						ResetSelectBR();

						CurrentGrid		= null;
						CurrentAction	= DoSelecting;
					}
				}
			}
		}

		protected void DoDrawMoveRange()
		{
			LockInput = true;

			async void m()
			{
				var grid = CurrentGrid;

				await CameraSystem.main.MoveTo(grid);
				MoveRange = new MoveRange(grid.Coordinate, 3);
				MoveRange.Draw(() =>
				{
					LockInput = false;
					CurrentAction = DoSelected;
				});
			}

			m();

			CurrentAction	= DoWaiting;
			CurrentGrid		= null;
		}

		protected void DoMoveTo()
		{
			var nodes = MapSystem.main.FindPath(ActionRole.Coordinate, CurrentGrid.Coordinate, MoveRange);
			if (nodes.Count > 0)
			{
				LockInput = true;
				ActionRole.PlayMoveTo(nodes, () =>
				{
					LockInput = false;
					CurrentAction = DoSelected;
				});

				CurrentAction = DoWaiting;
			}
			else
				CurrentAction = DoSelected;

			CurrentGrid = null;
		}

		protected void DoAttackTo()
		{
			LockInput = true;

			var target = RoleSystem.main.SelectBattleRole(CurrentGrid);
			ActionRole.PlayAttackTo(target, () =>
			{
				LockInput = false;
				CurrentAction = null;

				SetStateToEnd();
			});

			ResetSelectBR();

			CurrentAction	= DoWaiting;
			CurrentGrid		= null;
		}

		protected void ResetSelectGD()
		{
			if (SelectGrid != null)
			{
				SelectGrid.UnSelect();
				SelectGrid = null;
			}
		}

		protected void ResetSelectBR()
		{
			MoveRange.Clear();
			MoveRange = null;
			ActionRole = null;
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
		#endregion
	}
}