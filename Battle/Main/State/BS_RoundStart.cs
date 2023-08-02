using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Battle
{
	public class BS_RoundStart : BS_Base
	{
		public BS_RoundStart(BattleSystem bs) : base(bs)
		{

		}

		protected override void _OnEnter()
		{
			Debug.Log("[BS] Round Start");
			SetStateToEnd();
		}
	}
}