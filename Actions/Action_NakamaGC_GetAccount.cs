namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using Nakama;
	using Mutare.GameCreator.Nakama;
	using System.Threading.Tasks;
	using GameCreator.Variables;
#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class Action_NakamaGC_GetAccount : IAction
	{
		//public int example = 0;
		public bool GetFromCache = false; // if there is already an account was downloaded, take information from that.
		public VariableProperty statusCode = new VariableProperty();
		public List<GetAccount_Output> outputs = new List<GetAccount_Output>();

		public Actions onSuccess = null;
		public Actions onFail = null;

		// EXECUTABLE: ----------------------------------------------------------------------------

		public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            if (GetFromCache && NakamaManager.account != null)
            {
				ReturnData(NakamaManager.account, target);
				if (onSuccess != null) onSuccess.Execute(target);
			}
			Task<IApiAccount> task = NakamaManager.client.GetAccountAsync(NakamaManager.session);
			while (!task.IsCompleted) yield return null; // Scheint soweit fehlerfrei errors bewältigen zu können...
			try
			{
				NakamaManager.account = task.Result;
				ReturnData(task.Result, target);
				if (onSuccess != null) onSuccess.Execute(target);
			}
			catch (System.Exception e)
			{
				Debug.Log("Failed get Account data");
                if (onFail != null)onFail.Execute(target);
			}
			yield return 0;
		}

		private void ReturnData(IApiAccount account, GameObject target)
        {
			foreach(GetAccount_Output output in outputs)
            {
                switch (output.outputType)
                {
                    case Account_OutputType.CustomId:
						output.outputProperty.Set(account.CustomId, target);
                        break;
                    case Account_OutputType.Devices:
						//output.outputProperty.Set(account.Devices, target); <- Not implemented yet
						Debug.LogWarning("Account_OutputType.Devices is not Implemented yet");
						break;
                    case Account_OutputType.DisableTime:
						output.outputProperty.Set(account.DisableTime, target);
						break;
                    case Account_OutputType.Email:
						output.outputProperty.Set(account.Email, target);
						break;
                    case Account_OutputType.User_AppleId:
						output.outputProperty.Set(account.User.AppleId, target);
						break;
                    case Account_OutputType.User_AvatarUrl:
						output.outputProperty.Set(account.User.AvatarUrl, target);
						break;
                    case Account_OutputType.User_CreateTime:
						output.outputProperty.Set(account.User.CreateTime, target);
						break;
                    case Account_OutputType.User_DisplayName:
						output.outputProperty.Set(account.User.DisplayName, target);
						break;
                    case Account_OutputType.User_EdgeCount:
						output.outputProperty.Set(account.User.EdgeCount, target);
						break;
                    case Account_OutputType.User_FacebookId:
						output.outputProperty.Set(account.User.FacebookId, target);
						break;
                    case Account_OutputType.User_FacebookInstantGameId:
						output.outputProperty.Set(account.User.FacebookInstantGameId, target);
						break;
                    case Account_OutputType.User_GamecenterId:
						output.outputProperty.Set(account.User.GamecenterId, target);
						break;
                    case Account_OutputType.User_GoogleId:
						output.outputProperty.Set(account.User.GoogleId, target);
						break;
                    case Account_OutputType.User_Id:
						output.outputProperty.Set(account.User.Id, target);
						break;
                    case Account_OutputType.User_LangTag:
						output.outputProperty.Set(account.User.LangTag, target);
						break;
                    case Account_OutputType.User_Location:
						output.outputProperty.Set(account.User.Location, target);
						break;
                    case Account_OutputType.User_Metadata:
						output.outputProperty.Set(account.User.Metadata, target);
						break;
                    case Account_OutputType.User_Online:
						output.outputProperty.Set(account.User.Online, target);
						break;
                    case Account_OutputType.User_SteamId:
						output.outputProperty.Set(account.User.SteamId, target);
						break;
                    case Account_OutputType.User_Timezone:
						output.outputProperty.Set(account.User.Timezone, target);
						break;
                    case Account_OutputType.User_UpdateTime:
						output.outputProperty.Set(account.User.UpdateTime, target);
						break;
                    case Account_OutputType.User_Username:
						output.outputProperty.Set(account.User.Username, target);
						break;
                    default:
                        break;
                }
            }
			Debug.Log(account.Wallet);
        }
        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Nakama/Get Account";
		private const string NODE_TITLE = "Get Account {0}";


		private bool eFoldout_Status = false;
		private bool eFoldout_Output = false;
		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spGetFromCache;
		private SerializedProperty spStatusCode;
		private SerializedProperty spOutputs;
		private SerializedProperty spOnSuccess;
		private SerializedProperty spOnFail;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            if (GetFromCache)
            {
				return string.Format(NODE_TITLE, "from Cache");
			} else
            {
				return string.Format(NODE_TITLE, "from Nakama");
			}
			
		}

		protected override void OnEnableEditorChild ()
		{
			this.spGetFromCache = this.serializedObject.FindProperty("GetFromCache");
			this.spStatusCode = this.serializedObject.FindProperty("statusCode");
			this.spOutputs = this.serializedObject.FindProperty("outputs");
			this.spOnSuccess = this.serializedObject.FindProperty("onSuccess");
			this.spOnFail = this.serializedObject.FindProperty("onFail");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spGetFromCache = null;
			this.spStatusCode = null;
			this.spOutputs = null;
			this.spOnSuccess = null;
			this.spOnFail = null;
		}

		public override void OnInspectorGUI()
		{
			GUIStyle foldoutstyle = new GUIStyle(EditorStyles.foldout);
			foldoutstyle.margin.left = 10;

			this.serializedObject.Update();

			EditorGUILayout.HelpBox("Before this action can be fired, you have to authentificate", MessageType.Info, true);

			EditorGUILayout.PropertyField(this.spGetFromCache, new GUIContent("Get from Cache"));
            if (GetFromCache)
            {
				EditorGUILayout.HelpBox("If the account has already been loaded once, the information is taken from the cache.", MessageType.None, true);
            } else
            {
				EditorGUILayout.HelpBox("The request is reissued by the server, regardless of the cache.\nThe cache is overwritten with the current request.", MessageType.None, true);
			}

			GUIStyle padding = new GUIStyle();
			padding.margin.left = 18;
			padding.margin.right = 18;
			EditorGUILayout.BeginVertical(padding);
			EditorGUILayout.PropertyField(this.spOutputs, new GUIContent("Output"));
			EditorGUILayout.EndVertical();

			eFoldout_Status = EditorGUILayout.Foldout(eFoldout_Status, new GUIContent("Status", "The Status allow to react to certain Auth events..."), foldoutstyle);
			if (eFoldout_Status)
			{
				EditorGUILayout.PropertyField(spStatusCode, new GUIContent("Status"));
				EditorGUILayout.Space(10);
				EditorGUILayout.PropertyField(spOnSuccess, new GUIContent("On Success Action"));
				EditorGUILayout.PropertyField(spOnFail, new GUIContent("On Fail Action"));
				
			}
			this.serializedObject.ApplyModifiedProperties();
		}

		#endif

		[System.Serializable]
		public class GetAccount_Output
		{
			public Account_OutputType outputType;
			public VariableProperty outputProperty = new VariableProperty();
		}
		public enum Account_OutputType
		{
			CustomId,
			Devices,
			DisableTime,
			Email,
			User_AppleId,
			User_AvatarUrl,
			User_CreateTime,
			User_DisplayName,
			User_EdgeCount,
			User_FacebookId,
			User_FacebookInstantGameId,
			User_GamecenterId,
			User_GoogleId,
			User_Id,
			User_LangTag,
			User_Location,
			User_Metadata,
			User_Online,
			User_SteamId,
			User_Timezone,
			User_UpdateTime,
			User_Username
		}
	}
}
