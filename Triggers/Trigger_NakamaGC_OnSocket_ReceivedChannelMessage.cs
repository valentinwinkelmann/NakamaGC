namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using Nakama;
    using Mutare.GameCreator.Nakama;
    using GameCreator.Variables;
    [AddComponentMenu("")]
    public class Trigger_NakamaGC_OnSocket_ReceivedChannelMessage : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Nakama/Socket/ReceivedChannelMessage";
        public new static string COMMENT = "Receives realtime chat messages sent by other users.";
        //public new static bool REQUIRES_COLLIDER = true; // uncomment if the igniter requires a collider
#endif
        public StringProperty channelName = new StringProperty();
        public ChannelType channelType = ChannelType.Room;
        #region VariableProperties
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty ChannelId = new VariableProperty();
        [VariableFilter(Variable.DataType.Number)]
        public VariableProperty Code = new VariableProperty();
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty Content = new VariableProperty();
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty CreateTime = new VariableProperty();
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty GroupId = new VariableProperty();
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty MessageId = new VariableProperty();
        [VariableFilter(Variable.DataType.Bool)]
        public VariableProperty Persistent = new VariableProperty();
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty RoomName = new VariableProperty();
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty SenderId = new VariableProperty();
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty UpdateTime = new VariableProperty();
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty UserIdOne = new VariableProperty();
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty UserIdTwo = new VariableProperty();
        [VariableFilter(Variable.DataType.String)]
        public VariableProperty Username = new VariableProperty();
        #endregion
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
            triggerSocket.ReceivedChannelMessage += message =>
            {
                #region Output
                ChannelId.Set(message.ChannelId, gameObject); // string
                Code.Set(message.Code, gameObject); // int
                Content.Set(message.Content, gameObject); // string
                CreateTime.Set(message.CreateTime, gameObject); // string
                GroupId.Set(message.GroupId, gameObject); // string
                MessageId.Set(message.MessageId, gameObject); // string
                Persistent.Set(message.Persistent, gameObject); // bool
                RoomName.Set(message.RoomName, gameObject); // string
                SenderId.Set(message.SenderId, gameObject); // string
                UpdateTime.Set(message.UpdateTime, gameObject); // string
                UserIdOne.Set(message.UserIdOne, gameObject); // string
                UserIdTwo.Set(message.UserIdTwo, gameObject); // string
                Username.Set(message.Username, gameObject); // string
                #endregion
                isTrigger = true;
            };
            
            await triggerSocket.ConnectAsync(NakamaManager.session);
            NakamaManager.openChannels[name] = await triggerSocket.JoinChatAsync(channelName.GetValue(gameObject), channelType);
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