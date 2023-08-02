using EzTool;
using Slate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace ET.Battle
{
	public class PlayerSystem : MonoBehaviour
	{
		void Start() { }
		//void Update() { }

		public static int UID = 1;

		public async void PlayIdle(BattleRole br)
		{
			var player		= new SlatePlayer();
			var anime		= "Prefabs/Action/" + (br.IsPlayer ? "Castor" : "Target") + "-Idle.prefab";
			var gsCtrller	= await player.LoadAsync(anime);

			if (gsCtrller != null)
			{
				AddToParent(player);

				gsCtrller.name = "Idle_" + UID++;
				gsCtrller.SetCastor(br.gameObject);
				player.Play();
			}
		}

		public async void PlayMove(BattleRole br)
		{
			var player		= new SlatePlayer();
			var anime		= "Prefabs/Action/Actor-Run.prefab";
			var gsCtrller	= await player.LoadAsync(anime);

			if (gsCtrller != null)
			{
				AddToParent(player);

				gsCtrller.SetCastor(br.gameObject);
				player.Play();
			}
		}

		public async void PlayAttackTo(BattleRole castor, BattleRole target, Action onEnd)
		{
			var player		= new SlatePlayer();
			var anime		= "Prefabs/Action/Castor-Attack-01.prefab";
			var gsCtrller	= await player.LoadAsync(anime);

			if (gsCtrller != null)
			{
				AddToParent(player);

				gsCtrller.OSAction = new AttackToOSAction()
				{
					Castor = castor,
					Target = target
				};

				gsCtrller.SetCastor(castor.gameObject);
				gsCtrller.SetTarget(0, target.gameObject);
				player.Play(onEnd);
			}
		}

		public async void PlayHit(BattleRole br)
		{
			var player		= new SlatePlayer();
			var anime		= "Prefabs/Action/Actor-Hit.prefab";
			var gsCtrller	= await player.LoadAsync(anime);

			if (gsCtrller != null)
			{
				AddToParent(player);

				gsCtrller.SetCastor(br.gameObject);
				player.Play();
			}
		}

		public async void PlayHitBack(BattleRole br, Action onEnd)
		{
			var player		= new SlatePlayer();
			var anime		= "Prefabs/Action/Actor-Hit-Back.prefab";
			var gsCtrller	= await player.LoadAsync(anime);

			if (gsCtrller != null)
			{
				AddToParent(player);

				gsCtrller.SetCastor(br.gameObject);
				player.Play(onEnd);
			}
		}

		public async Task<ParticleSystem> PlayParticleSystem(string file, Vector2Int pos)
		{
			var handle		= Addressables.LoadAssetAsync<GameObject>(file);
			var partical	= default(ParticleSystem);

			await handle.Task;
			if (handle.Result != null)
			{
				var effect = GameObject.Instantiate(handle.Result);
				partical = effect.GetComponent<ParticleSystem>();

				MapSystem.main.SetActorToGrid(effect, pos);
				AddToParent(partical);

				partical.Play();
			}

			Addressables.Release(handle);
			return partical;
		}

		protected void AddToParent(SlatePlayer player)
		{
			player.GSCtrller.transform.SetParent(gameObject.transform);	
		}

		protected void AddToParent(ParticleSystem ps)
		{
			ps.gameObject.transform.SetParent(gameObject.transform);
		}

		protected static PlayerSystem _main = null;
		public static PlayerSystem main
		{
			get
			{
				if (_main == null)
				{
					var go = new GameObject("PlayerSystem");
					_main = go.AddComponent<PlayerSystem>();

					go.transform.SetParent(BattleSystem.main.transform);
					go.transform.localPosition	= Vector3.zero;
					go.transform.localRotation	= Quaternion.identity;
					go.transform.localScale		= Vector3.one;
				}

				return _main;
			}
		}



		#region GSOutsideAction

		public class AttackToOSAction : GSOutsideAction
		{
			public BattleRole Castor = null;
			public BattleRole Target = null;

			public override void ExecuteAction(Cutscene sender, string action, bool isSpString = false)
			{
				if (!isSpString) Debug.Log(string.Format("SendAction: {0}", action));
				else
				{
					var sp = new SpData(action);
					var act = sp.Get<string>("Type");

					if (act == "Hit")
					{
						Target.PlayHit();
					}
					else if (act == "Hit-Back")
					{
						Target.PlayHitBack(Castor);
					}
				}
			}
		}
		#endregion
	}
}