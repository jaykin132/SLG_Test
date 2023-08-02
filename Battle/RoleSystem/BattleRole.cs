using EzTool;
using Slate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEngine.GraphicsBuffer;

namespace ET.Battle
{
	public class BattleRole
	{
		public bool			IsPlayer	= true;
		public bool			IsNpc		= false;
		public GameObject	gameObject	= null;
		public Vector2Int	Coordinate	= Vector2Int.one;
		public Vector3		Forward		= Vector3.forward;
		public bool			CanAction	= true;
		public bool			IsDead		= false;

		public async Task AppearToMap(float delay)
		{
			// appear one by one by delay
			await Task.Delay((int)(delay * 1000));

			// appear effect
			var effectFile	= "Prefabs/Appear.prefab";
			var partical    = await PlayerSystem.main.PlayParticleSystem(effectFile, Coordinate);

			if (partical != null)
			{
				await Task.Delay(500);
				partical.Stop(true);

				AutoRun.Create()
					.Where( _ => !partical.IsAlive() )
					.Do( _ => GameObject.Destroy(partical.gameObject) )
					.Start();
			}

			// appear role
			gameObject.SetActive(true);
			MapSystem.main.SetBattleRoleToGrid(this);

			PlayIdle();
		}

		public async Task FocusMe()
		{
			var pos = gameObject.transform.position;
			await CameraSystem.main.MoveTo(pos);
		}

		public void PlayIdle()
		{
			PlayerSystem.main.PlayIdle(this);
		}

		public async void PlayMoveTo(List<GridData> nodes, Action onEnd)
		{
			var last    = nodes[^1];
			var idx		= 1;

			while (nodes.Count > 0)
			{
				if (idx == 1)
					PlayerSystem.main.PlayMove(this);

				var current	= nodes[idx++];
				var isLast	= current == last;

				gameObject.transform.LookAt(current.Center);

				var offset = 0.25f;
				while(true)
				{
					var check = (gameObject.transform.position - current.Center).magnitude;
					if (check < offset)
					{
						if (isLast)
						{
							//onApproachingTarget?.Invoke();
							PlayIdle();

							var from = gameObject.transform.position;
							var to = current.Center;
							var t = 0f;
							var tt = 0.25f;

							while (true)
							{
								t += BattleSystem.main.GameDeltaTime;
								gameObject.transform.position = Slate.Easing.Ease(Slate.EaseType.Linear, from, to, Mathf.Min(t / tt, 1f));

								if (t >= tt)
								{
									RoleSystem.main.UpdateBattleRoleCoordinate(this, last.Coordinate);
									break;
								}

								await Task.Yield();
							}
						}

						break;
					}

					await Task.Yield();
				}

				if (current == last)
					break;
			}
			
			onEnd?.Invoke();
		}	

		public void PlayAttackTo(BattleRole target, Action onEnd)
		{
			gameObject.transform.LookAt(target.gameObject.transform.position);
			PlayerSystem.main.PlayAttackTo(this, target, onEnd);
		}

		public void PlayHit()
		{
			PlayerSystem.main.PlayHit(this);
		}

		public void PlayHitBack(BattleRole castor)
		{
			gameObject.transform.LookAt(castor.gameObject.transform.position);
			PlayerSystem.main.PlayHitBack(this, () =>
			{
				// update hit back coordinate
				var grid = MapSystem.main.PositionToGrid(gameObject.transform.position);
				RoleSystem.main.UpdateBattleRoleCoordinate(this, grid.Coordinate);
			});
		}

		public static BattleRole Create(Vector2Int pos, Vector3 forward, GameObject gameObject)
		{
			return Create(pos.x, pos.y, forward, gameObject);
		}

		public static BattleRole Create(int x, int y, Vector3 forward, GameObject gameObject)
		{
			var br = new BattleRole
			{
				gameObject = gameObject,
				Coordinate = new Vector2Int(x, y),
				Forward = forward
			};

			br.gameObject.SetActive(false);
			return br;
		}
	}
}