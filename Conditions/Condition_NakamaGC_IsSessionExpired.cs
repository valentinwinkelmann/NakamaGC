namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator;
	using GameCreator.Variables;
	using Nakama;
	using Mutare.GameCreator.Nakama;
	[AddComponentMenu("")]
	public class Condition_NakamaGC_IsSessionExpired : ICondition
	{
		public StringProperty authTokken = new StringProperty();
		public override bool Check(GameObject target)
		{
            if (Session.Restore(authTokken.GetValue(target)).IsExpired)
            {
				return true;
            } else
            {
				return false;
            }
		}
        
		#if UNITY_EDITOR
        public static new string NAME = "Nakama/Is Session Expired";
		#endif
	}
}
