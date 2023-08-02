using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Slate;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace ET.Battle
{
	public class SlatePlayer
	{
		public GSController GSCtrller = null;

		public Cutscene Cutscene => GSCtrller != null ? GSCtrller.Cutscene: null;

		public async Task<GSController> LoadAsync(string path)
		{
			var handle = Addressables.LoadAssetAsync<GameObject>(path);
			await handle.Task;

			if (handle.Result != null)
			{
				var go = GameObject.Instantiate(handle.Result);
				
				GSCtrller = go.GetComponent<GSController>();
				GSCtrller.IsGameMode = true;
			}
			else GSCtrller = null;

			Addressables.Release(handle);
			return GSCtrller;
		}

		public void Play(System.Action onStop = null)
		{
			if (Cutscene != null)
			{
				Cutscene.Play( () =>
				{
					onStop?.Invoke();
					GameObject.Destroy(Cutscene.gameObject);
					GSCtrller = null;
				});
			}
		}
	}
}