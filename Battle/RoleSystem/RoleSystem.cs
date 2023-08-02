using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ET.Battle
{
	public enum RsSide
	{
		Player = 0,
		PlayerNpc,
		Enemy,
	}

	public class RoleSystem
	{
		public RsSide CurrentSide = RsSide.Player;

		public Dictionary<Vector2Int, BattleRole>	BattleRoles = new();
		public List<BattleRole>						PlayerList	= new();
		public List<BattleRole>						PNpcList	= new();
		public List<BattleRole>						EnemyList	= new();

		public int PlayerIdx = 0;
		public int PNpcIdx	 = 0;
		public int EnemyIdx	 = 0;

		protected LinkedList<BattleRole> PlayerCanActionList = new();
		protected LinkedList<BattleRole> PNpcCanActionList   = new();
		protected LinkedList<BattleRole> EnemyCanActionList  = new();

		public async void CreateRoles(System.Action onRoleCreated = null)
		{
			PlayerIdx = 0;
			EnemyIdx  = 0;

			PlayerList.Clear();
			EnemyList.Clear();

			var handle = Addressables.LoadAssetAsync<GameObject>("Prefabs/Samurai-Castor.prefab");
			await handle.Task;

			if (handle.Result != null)
			{
				var pPoses = new Vector2Int[]
				{
					new Vector2Int(6, 8),
					new Vector2Int(8, 8),
					new Vector2Int(10, 8),
				};

				var ePoses = new Vector2Int[]
				{
					new Vector2Int(7, 10),
					new Vector2Int(9, 11),
				};

				// Player
				for (int i = 0; i < pPoses.Length; i++)
				{
					var actor		= GameObject.Instantiate(handle.Result);
					var pos			= pPoses[i];
					var br			= BattleRole.Create(pos, Vector3.forward, actor);

					br.gameObject.name  = "Player_" + i;
					br.IsPlayer			= true;
					BattleRoles[pos]	= br;

					PlayerList.Add(br);
					PlayerCanActionList.AddLast(br);

					await CameraSystem.main.MoveTo(pos);
					await br.AppearToMap(0f);
				}

				// Enemy
				for (int i = 0; i < ePoses.Length; i++)
				{
					var actor		= GameObject.Instantiate(handle.Result);
					var pos			= ePoses[i];
					var br			= BattleRole.Create(pos, Vector3.back, actor);

					br.gameObject.name  = "Enemy_" + i;
					br.IsPlayer			= false;
					BattleRoles[pos]	= br;

					EnemyList.Add(br);
					EnemyCanActionList.AddLast(br);

					await CameraSystem.main.MoveTo(pos);
					await br.AppearToMap(0f);
				}

				Addressables.Release(handle);

				// Camera Focus first player
				await PlayerList[0].FocusMe();
			}

			onRoleCreated?.Invoke();
		}

		public void UpdateBattleRoleCoordinate(BattleRole br, Vector2Int pos)
		{
			BattleRoles.Remove(br.Coordinate);
			BattleRoles[pos] = br;
			br.Coordinate = pos;
		}

		public void SetRoleActionDone(BattleRole br)
		{
			br.CanAction = false;

			if (br.IsPlayer)
			{
				if (!br.IsNpc)
					PlayerCanActionList.Remove(br);
				else
					PNpcCanActionList.Remove(br);
			}
			else
			{
				EnemyCanActionList.Remove(br);
			}
		}

		public bool IsAllRoleCanAction()
		{
			return	PlayerCanActionList.Count == PlayerList.Count &&
					PNpcCanActionList.Count == PNpcList.Count	  &&
					EnemyCanActionList.Count == EnemyList.Count;
		}

		public bool IsAllRoleActionDone()
		{
			return	PlayerCanActionList.Count == 0 && 
					PNpcCanActionList.Count == 0 &&
					EnemyCanActionList.Count == 0;
		}

		public bool IsSideActionDone()
		{
			var count = CurrentSide switch
			{
				RsSide.Player		=> PlayerCanActionList.Count,
				RsSide.PlayerNpc	=> PNpcCanActionList.Count,
				RsSide.Enemy		=> EnemyCanActionList.Count,
				_					=> 0,
			};

			return count == 0;
		}

		public void ChangeSide()
		{
			++CurrentSide;

			if (CurrentSide > RsSide.Enemy)
				CurrentSide = RsSide.Player;
		}

		public bool IsPlayerAllDead()
		{
			var ret = true;

			foreach (var br in PlayerList)
			{
				if (br.IsDead == false)
				{
					ret = false;
					break;
				}
			}

			return ret;
		}

		public bool IsEnemyAllDead()
		{
			var ret = true;

			foreach (var br in EnemyList)
			{
				if (br.IsDead == false)
				{
					ret = false;
					break;
				}
			}

			return ret;
		}

		public bool HaveBattleRole(GridData grid)
		{
			return HaveBattleRole(grid.X, grid.Y);
		}

		public bool HaveBattleRole(int x, int y)
		{
			return SelectBattleRole(x, y) != null;
		}

		public BattleRole SelectBattleRole(GridData grid)
		{
			return SelectBattleRole(grid.X, grid.Y);
		}

		public BattleRole SelectBattleRole(int x, int y)
		{
			var pos = new Vector2Int(x, y);

			BattleRoles.TryGetValue(pos, out var br);
			return br;
		}

		public void Destroy()
		{
			foreach (var br in BattleRoles.Values)
			{
				GameObject.Destroy(br.gameObject);
			}

			BattleRoles.Clear();
			_main = null;
		}

		protected List<BattleRole> GetRoleList(RsSide side)
		{
			var list = side switch
			{
				RsSide.Player		=> PlayerList,
				RsSide.PlayerNpc	=> PNpcList,
				RsSide.Enemy		=> EnemyList,
				_					=> null
			};

			return list;
		}

		protected static RoleSystem _main = null;
		public static RoleSystem main
		{
			get
			{
				if (_main == null)
				{
					_main = new RoleSystem();
				}
				return _main;
			}
		}
	}
}