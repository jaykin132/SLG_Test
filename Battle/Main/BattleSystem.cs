using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzTool;

namespace ET.Battle
{
	public class BattleSystem : MonoBehaviour
	{
		public static float deltaTime = 0f;

		public const string BS_MapInit		= "BS_MapInit";
		public const string BS_MapCreate	= "BS_MapCreate";
		public const string BS_EventHandle  = "BS_EventHandle";
		public const string BS_CheckFlow	= "BS_CheckFlow";
		public const string BS_RoleCreate	= "BS_RoleCreate";
		public const string BS_RoundStart	= "BS_RoundStart";
		public const string BS_RoundEnd		= "BS_RoundEnd";
		public const string BS_CheckSide	= "BS_CheckSide";
		public const string BS_PlayerInput	= "BS_PlayerInput";
		public const string BS_Victory		= "BS_Victory";
		public const string BS_Lose			= "BS_Lose";
		public const string BS_End 			= "BS_End";	

		public float  GameDeltaTime		= 0.0f;
		public string LastState 		= "";
		public string NestState			= "";
		public string RoleSetupTable	= "";

		protected FSM _FSM = null;

		public void Init()
		{
			_FSM = new();

			_FSM[BS_MapInit]		= new BS_MapInit(this);
			_FSM[BS_MapCreate]		= new BS_MapCreate(this);
			_FSM[BS_EventHandle]	= new BS_EventHandle(this);
			_FSM[BS_CheckFlow]		= new BS_CheckFlow(this);
			_FSM[BS_RoleCreate]		= new BS_CreateRole(this);
			_FSM[BS_RoundStart]		= new BS_RoundStart(this);
			_FSM[BS_CheckSide]		= new BS_CheckSide(this);
			_FSM[BS_PlayerInput]	= new BS_PlayerInput(this);

			// BS_MapInit to State
			_FSM[BS_MapInit].To(BS_MapCreate).To(BS_EventHandle).To(BS_CheckFlow);

			// BS_CheckFlow to State
			_FSM[BS_CheckFlow]
				.If( _ => NestState == BS_RoleCreate ).To(BS_RoleCreate).To(BS_EventHandle);

			_FSM[BS_CheckFlow]
				.If(_ => NestState == BS_RoundStart).To(BS_RoundStart).To(BS_CheckSide);

			//_FSM[BS_CheckFlow]
			//	.If( _ => FlowToState == BS_CheckSide ).To(BS_CheckSide);

			//_FSM[BS_CheckFlow]
			//	.If( _ => FlowToState == BS_Victory ).To(BS_Victory).To(BS_End);

			//_FSM[BS_CheckFlow]
			//	.If( _ => FlowToState == BS_Lose ).To(BS_Lose).To(BS_End);

			//
			_FSM[BS_CheckSide]
				.If( _ => NestState == BS_PlayerInput).To(BS_PlayerInput).To(BS_EventHandle);


			_FSM.Start(BS_MapInit);
		}

		void Start()
		{
		}
		
		void Update()
		{
			deltaTime = Time.deltaTime;

			if (_FSM != null)
			{
				GameDeltaTime = Time.deltaTime;
				_FSM.Update(GameDeltaTime);

				if (_FSM.State == FSM.eState.End)
				{
					_FSM = null;
				}
			}
		}

		protected static BattleSystem _main = null;
		public static BattleSystem main
		{
			get
			{
				if (_main == null)
				{
					var go = new GameObject("BattleSystem");
					_main = go.AddComponent<BattleSystem>();

					go.transform.localPosition	= Vector3.zero;
					go.transform.localRotation	= Quaternion.identity;
					go.transform.localScale		= Vector3.one;
				}

				return _main;
			}
		}
	}
}