using EzTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ET.Battle
{
	public class BS_MapCreate : BS_Base
	{
		public BS_MapCreate(BattleSystem bs) : base(bs)
		{

		}

		protected override void _OnEnter()
		{
			Debug.Log("[BS] MapSystem Create");

			MapSystem.main.Load();
			MapSystem.main.MapData.IsEditor = false;

			//Camera.main.transform.position		= new Vector3(6.812954f, 10.08835f, -1.646568f);
			//Camera.main.transform.eulerAngles	= new Vector3(54.591f, -0.297f, 0.059f);

			AutoRun.Create()
				.Wait( _ => MapDrawer.main.CreateMapAutoRun(MapSystem.main.MapData) )
				.Do( _ =>
				{
					//CameraSystem.main.Init();
					//await CameraSystem.main.MoveTo(9, 9);
					SetStateToEnd();
				})
				.Start();

			//async void m()
			//{
			//	await MapDrawer.main.CreateMapAsync(MapSystem.main.MapData);

			//	CameraSystem.main.Init();
			//	await CameraSystem.main.MoveTo(9, 9);
			//	SetStateToEnd();
			//}

			//m();
		}
	}
}