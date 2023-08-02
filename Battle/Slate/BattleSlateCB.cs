using UnityEngine;
using Slate;
using UnityEngine.SceneManagement;
using System.Linq;
using EzTool;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ET.Battle
{
	public class BattleSlateCB
	{
		public static void Register()
		{
			// GSCastorGroup
			GSCallBack.DoCastorGroupValidate	= BattleSlateCB.DoCastorGroupValidate;
			GSCallBack.AfterCastorGroupEnter	= BattleSlateCB.AfterCastorGroupEnter;
			GSCallBack.AfterCastorGroupReset	= BattleSlateCB.AfterCastorGroupReset;
			GSCallBack.AfterCastorGroupReverse	= BattleSlateCB.AfterCastorGroupReverse;
			
			// GSTargetGroup
			GSCallBack.AfterTargetGroupReset	= BattleSlateCB.AfterTargetGroupReset;
			GSCallBack.AfterTargetGroupEnter	= BattleSlateCB.AfterTargetGroupEnter;
			GSCallBack.AfterTargetGroupReverse	= BattleSlateCB.AfterTargetGroupReverse;
			GSCallBack.GetTargetHitCutscene		= BattleSlateCB.GetTargetHitCutscene;
		}

		#region GSCastorGroup
		public static void DoCastorGroupValidate(GSCastorGroup group)
		{
			//if (group.GSCtrller == null || group.GSCtrller.IsEditorMode)
			//	AdjustCastorCutsceneInitPosition(group);
		}

		public static void AfterCastorGroupReset(GSCastorGroup group)
		{
			var sp = group.SerializedData.SpData();

			sp.Set("X", 4);
			sp.Set("Y", 4);
		}

		public static void AfterCastorGroupEnter(GSCastorGroup group)
		{
			if (group.actor == null) return;

			var sp		= group.SerializedData.SpData();
			var xGrid	= sp.Get<int>("X");
			var yGrid	= sp.Get<int>("Y");
			var pos		= MapSystem.main.GetGridCenter(xGrid, yGrid);
			var actor	= group.actor;
			var gs		= group.GSCtrller;

			// 先設定 Castor 的位置
			if (gs != null)
			{
				if (gs.IsEditorMode)
				{
					actor.transform.position = group.Cutscene.transform.position;
					actor.transform.forward = group.Cutscene.transform.forward;
				}
				else
				{
					var ct = gs.Cutscene;

					ct.transform.position = actor.transform.position;
					ct.transform.forward = actor.transform.forward;
				}

				var cloned = gs.Replacer.GetActor(GSReplacer.eKey.Actor1) == null;
				if (cloned)
				{ // rename

					actor.name = "Castor";
					gs.SetActorToContainer(actor);
				}
			}
		}

		public static void AfterCastorGroupReverse(GSCastorGroup group)
		{
			var ct = group.Cutscene;
			if (ct != null)
			{
				//var sp = group.SerializedData.SpData();
				//var rm1 = sp.Remove<Vector3>("CP", out var p);
				//var rm2 = sp.Remove<Vector3>("CF", out var f);

				//if (rm1 && rm2)
				//{
				//	ct.transform.position = p;
				//	ct.transform.forward = f;
				//}

				var gs = group.GSCtrller;
				if (gs != null && gs.IsEditorMode)
				{
					gs.RemoveActorContainer();
				}
			}
		}

		public static void AdjustCastorCutsceneInitPosition(GSCastorGroup group)
		{
			var spData = group.SerializedData.SpData();
			var grid_x = spData.Get<int>("X");
			var grid_y = spData.Get<int>("Y");

			AdjustCastorCutsceneInitPosition(group, grid_x, grid_y);
		}

		public static void AdjustCastorCutsceneInitPosition(GSCastorGroup group, int grid_x, int grid_y)
		{
			group.initialLocalPosition = Vector3.zero;
			group.root.context.transform.position = MapSystem.main.GetGridCenter(grid_x, grid_y);
		}
		#endregion

		#region GSTargetGroup
		public static void AfterTargetGroupReset(GSTargetGroup group)
		{
			var sp = group.SerializedData.SpData();
			sp.Set(0, new Vector2(4, 3));
		}

		public static void AfterTargetGroupEnter(GSTargetGroup group)
		{
			var gs = group.GSCtrller;
			var isAction = gs != null && gs.IsEditorMode;

			if (isAction)
			{
				var first = group.actor;

				gs.ClearTargets();

				if (first != null)
				{
					gs.SetTarget(0, first);
					gs.SetActorToContainer(first);
				}

				if (group.Count > 1)
				{
					for (int i = 1; i < group.Count; ++i)
					{
						var ator = GameObject.Instantiate(first);
						SceneManager.MoveGameObjectToScene(ator, group.root.context.scene);
						ator.transform.SetParent(first.transform.parent, false);

						if (!ator.activeSelf)
							ator.SetActive(true);

						gs.SetTarget(i, ator);
						gs.SetActorToContainer(ator);
					}
				}

				// rename name
				for (int i = 0; i < group.Count; ++i)
				{
					var ator = gs.GetTarget(i);
					ator.name = group.Name(i);
				}

				// set position
				if (MapSystem.main != null)
				{
					for (int i = 0; i < group.Count; ++i)
					{
						var ator = gs.GetTarget(i);
						var sp = group.SerializedData.SpData();
						var pos = sp.Get<Vector2>(i);

						ator.transform.position = MapSystem.main.GetGridCenter((int)pos.x, (int)pos.y);
					}
				}
			}
		}

		public static void AfterTargetGroupReverse(GSTargetGroup group)
		{
			var gs = group.GSCtrller;
			var isAction = gs != null && gs.IsEditorMode;

			if (isAction)
			{
				var count = group.Count;
				for (int i = 1; i < count; ++i)
				{
					var ator = gs.GetTarget(i);
					EZM.Destroy(ator);
				}

				gs.ClearTargets();
				gs.RemoveActorContainer();
			}
		}

		public static Cutscene GetTargetHitCutscene(GSController sender, int targetIdx, GameObject target)
		{
			var actor	= target != null ? target : sender.GetTarget(targetIdx);
			var ret		= default(Cutscene);

			if (actor != null)
			{
				var ActorHitPath = "Assets/ResourceAB/Prefabs/Action/Actor-Hit.prefab";

#if UNITY_EDITOR
				var ActorHit = AssetDatabase.LoadAssetAtPath<GameObject>(ActorHitPath);
#else
			var ActorHit = null;
#endif

				if (ActorHit != null)
				{
					var go = GameObject.Instantiate(ActorHit);
					var ctrller = go.GetComponent<GSController>();

					ctrller.IsGameMode = true;
					ctrller.SetCastor(actor);

					ret = go.GetComponent<Cutscene>();
					ret.Validate();
				}
			}

			return ret;
		}
		#endregion
	}
}