namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using Nakama;
    using Mutare.GameCreator.Nakama;
    using GameCreator.Variables;
    [AddComponentMenu("")]
    public class Trigger_NakamaGC_OnSocket_ReceivedNotification : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Nakama/Socket/ReceivedNotification";
        public new static string COMMENT = "Receives live in-app notifications sent from the server.";
        //public new static bool REQUIRES_COLLIDER = true; // uncomment if the igniter requires a collider
#endif
        public VariableProperty Code = new VariableProperty();
        public VariableProperty Content = new VariableProperty();
        public VariableProperty CreateTime = new VariableProperty();
        public VariableProperty Id = new VariableProperty();
        public VariableProperty Persistent = new VariableProperty();
        public VariableProperty SenderId = new VariableProperty();
        public VariableProperty Subject = new VariableProperty();

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
            triggerSocket.ReceivedNotification += message =>
            {
                Code.Set(message.Code, gameObject);
                Content.Set(message.Content, gameObject);
                CreateTime.Set(message.CreateTime, gameObject);
                Id.Set(message.Id, gameObject);
                Persistent.Set(message.Persistent, gameObject);
                SenderId.Set(message.SenderId, gameObject);
                Subject.Set(message.Subject, gameObject);
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