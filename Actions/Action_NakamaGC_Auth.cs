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
	public class Action_NakamaGC_Auth : IAction
	{
		public StringProperty email = new StringProperty();
		public StringProperty password = new StringProperty();
		public BoolProperty create = new BoolProperty(false);

		public VariableProperty statusCode = new VariableProperty();

		public VariableProperty user_AuthTokken = new VariableProperty();
		public VariableProperty user_UserId = new VariableProperty();
		public VariableProperty user_Username = new VariableProperty();
		public VariableProperty user_Created = new VariableProperty();
		public VariableProperty user_CreateTime = new VariableProperty();
		public VariableProperty user_IsExpired = new VariableProperty();

		public VariableProperty tokken = new VariableProperty();
		private bool isDone = false;
		private ISession session;
		public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
			Task<ISession> task = NakamaManager.client.AuthenticateEmailAsync(
					email.GetValue(target),
					password.GetValue(target),
					email.GetValue(target),
					create.GetValue(target)
				);
			while (!task.IsCompleted) yield return null; // Scheint soweit fehlerfrei errors bewältigen zu können...
            try
            {
				statusCode.Set(0, target);

				tokken.Set(task.Result.AuthToken, target);
				user_UserId.Set(task.Result.UserId, target);
				user_Username.Set(task.Result.Username, target);
				user_Created.Set(task.Result.Created, target);
				user_CreateTime.Set(task.Result.CreateTime, target);
				user_IsExpired.Set(task.Result.IsExpired, target);
			}
            catch(System.Exception e)
            {
				if (e.InnerException is ApiResponseException) // Nakama Errors
				{
					Debug.Log("API ERROR");
					Debug.Log((e.InnerException as ApiResponseException).StatusCode);
					Debug.Log((e.InnerException as ApiResponseException).Message);
					var exc = (e.InnerException as ApiResponseException);
					switch (exc.StatusCode)
					{
						case 400: // Password must be at least 8 characters long.
							statusCode.Set(5, target);
							break;
						case 401: // Invalid credentials or invalid server key.
							if(exc.Message == "Server key invalid")
                            {
								statusCode.Set(4, target);
								break;
							}
							statusCode.Set(1, target);
							break;
						case 404: // User account not found.
							statusCode.Set(2, target);
							break;
					}
				}
				else
				{ // Any other Erros like Internet connection, firewall etc...
				  //Debug.Log("ANY ERROR");
				  //Debug.Log(e.InnerException);
					if (e.InnerException is System.Net.Http.HttpRequestException) // No internet Connection
					{
						statusCode.Set(3, target);
					}
				}
            }

			yield return 0;
		}
#if UNITY_EDITOR

		private bool eFoldout_Outputs = false;
		private bool eFoldout_Status = false;
		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spEmail;
		private SerializedProperty spPassword;
		private SerializedProperty spCreate;

		private SerializedProperty spStatusCode;

		private SerializedProperty spuser_AuthTokken;
		private SerializedProperty spuser_UserId;
		private SerializedProperty spuser_Username;
		private SerializedProperty spuser_Created;
		private SerializedProperty spuser_CreateTime;
		private SerializedProperty spuser_IsExpired;

		public static new string NAME = "Nakama/Authenticate";
		public override string GetNodeTitle()
		{
			return "Nakama Auth";
		}

		protected override void OnEnableEditorChild()
		{
			this.spEmail = this.serializedObject.FindProperty("email");
			this.spPassword = this.serializedObject.FindProperty("password");
			this.spCreate = this.serializedObject.FindProperty("create");
			this.spStatusCode = this.serializedObject.FindProperty("statusCode");
			this.spuser_AuthTokken = this.serializedObject.FindProperty("user_AuthTokken");
			this.spuser_UserId = this.serializedObject.FindProperty("user_UserId");
			this.spuser_Username = this.serializedObject.FindProperty("user_Username");
			this.spuser_Created = this.serializedObject.FindProperty("user_Created");
			this.spuser_CreateTime = this.serializedObject.FindProperty("user_CreateTime");
			this.spuser_IsExpired = this.serializedObject.FindProperty("user_IsExpired");
		}

		protected override void OnDisableEditorChild()
		{
			this.spEmail = null;
			this.spPassword = null;
			this.spCreate = null;
			this.spStatusCode = null;
			this.spuser_AuthTokken = null;
			this.spuser_UserId = null;
			this.spuser_Username = null;
			this.spuser_Created = null;
			this.spuser_CreateTime = null;
			this.spuser_IsExpired = null;
		}

		public override void OnInspectorGUI()
		{
			GUIStyle foldoutstyle = new GUIStyle(EditorStyles.foldout);
			foldoutstyle.margin.left = 10;


			this.serializedObject.Update();
			EditorGUILayout.LabelField("Inputs", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(this.spEmail, new GUIContent("E-Mail"));
			EditorGUILayout.PropertyField(this.spPassword, new GUIContent("Password"));
			EditorGUILayout.PropertyField(this.spCreate, new GUIContent("Create new User", "If no user exists, schould a new be created?"));
			EditorGUILayout.Space(10);

			eFoldout_Status = EditorGUILayout.Foldout(eFoldout_Status, new GUIContent("Status", "The Status allow to react to certain Auth events..."), foldoutstyle);
            if (eFoldout_Status)
            {
				EditorGUILayout.PropertyField(this.spStatusCode, new GUIContent("Status Code"));
				EditorGUILayout.HelpBox("Response codes are written in variables and allow to react to certain Auth events...\n" +
					"0:Success\n" +
					"1:Wrong Creditials\n" +
					"2:User not Found\n" +
					"3:no Connection\n" +
					"4:wrong serverkey\n" +
					"5:Password is to short"
				, MessageType.Info, true);
			}


			eFoldout_Outputs = EditorGUILayout.Foldout(eFoldout_Outputs, new GUIContent("Outputs", "Optional"), foldoutstyle);
            if (eFoldout_Outputs)
            {
				EditorGUILayout.PropertyField(this.spuser_AuthTokken, new GUIContent("Auth Tokken"));
				EditorGUILayout.PropertyField(this.spuser_UserId, new GUIContent("User ID"));
				EditorGUILayout.PropertyField(this.spuser_Username, new GUIContent("Username"));
				EditorGUILayout.PropertyField(this.spuser_Created, new GUIContent("Created"));
				EditorGUILayout.PropertyField(this.spuser_CreateTime, new GUIContent("Created Time"));
				EditorGUILayout.PropertyField(this.spuser_IsExpired, new GUIContent("IsExpired"));
			}

			this.serializedObject.ApplyModifiedProperties();
		}

#endif
	}
}
