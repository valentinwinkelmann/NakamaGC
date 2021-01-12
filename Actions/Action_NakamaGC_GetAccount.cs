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

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
			Task<IApiAccount> task = NakamaManager.client.GetAccountAsync(NakamaManager.session);
			while (!task.IsCompleted) yield return null; // Scheint soweit fehlerfrei errors bewältigen zu können...
			try
			{
				Debug.Log(task.Result.Email);
			}
			catch (System.Exception e)
			{
				Debug.Log("Failed get Account data");
			}
			}

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Nakama/Get Account";
		private const string NODE_TITLE = "Get Account {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spExample;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, "INFO");
		}

		protected override void OnEnableEditorChild ()
		{
			//this.spExample = this.serializedObject.FindProperty("example");
		}

		protected override void OnDisableEditorChild ()
		{
			//this.spExample = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.HelpBox("Before this action can be fired, you have to authentificate", MessageType.Info, true);

			//EditorGUILayout.PropertyField(this.spExample, new GUIContent("Example Value"));

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
