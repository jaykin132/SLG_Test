using System;
using UnityEngine;
using EzTool;

namespace Slate.ActionClips
{
	[Category("Send Action")]
	[Name("Hit Back¡iGS¡j")]
	[Attachable(typeof(GSTargetActionTrack), typeof(GSActorActionTrack), typeof(ActorActionClip))]
	public class GSHitBackClip : GSSendActionClip
	{
		public override string info => "Hit-Back";

		protected override void OnCreate()
		{
			base.OnCreate();
		
			SetType("Hit-Back");
			SetEnd();
		}
	}
}
