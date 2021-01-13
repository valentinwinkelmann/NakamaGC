namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using Nakama;
    using Mutare.GameCreator.Nakama;
    using GameCreator.Variables;
    [AddComponentMenu("")]
    public class Trigger_NakamaGC_OnSocket_ReceivedError : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Nakama/Socket/ReceivedError";
        public new static string COMMENT = "Receives events about server errors.";
        //public new static bool REQUIRES_COLLIDER = true; // uncomment if the igniter requires a collider
#endif
        public VariableProperty Message = new VariableProperty();
        public VariableProperty StackTrace = new VariableProperty();
        public VariableProperty Source = new VariableProperty();

        private bool isTrigger = false;
        private ISocket triggerSocket;
        private void Start()
        {
            NakamaManager.OnSession_Connect.AddListener(() =>
            {
                this.Init();
            });
        }
        private async void Init() // Initialize Socket listener
        {
            triggerSocket = NakamaManager.client.NewSocket();
            triggerSocket.ReceivedError += message =>
            {
                Message.Set(message.Message, gameObject);
                StackTrace.Set(message.StackTrace, gameObject);
                Source.Set(message.Source, gameObject);
                isTrigger = true;
            };
            await triggerSocket.ConnectAsync(NakamaManager.session);
        }
        private void Update()
        {
            if (this.isTrigger)
            {
                this.ExecuteTrigger(gameObject);
                this.isTrigger = false;
            }
        }
    }
}