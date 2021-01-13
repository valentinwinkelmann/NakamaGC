namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using Nakama;
    using Mutare.GameCreator.Nakama;

    [AddComponentMenu("")]
    public class Trigger_NakamaGC_OnSocket_Closed : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Nakama/Socket/Closed";
        public new static string COMMENT = "Receives an event for when the client is disconnected from the server.";
        //public new static bool REQUIRES_COLLIDER = true; // uncomment if the igniter requires a collider
#endif

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
            triggerSocket.Closed += () =>
            {
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