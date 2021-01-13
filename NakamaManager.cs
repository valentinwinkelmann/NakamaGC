using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Nakama;
namespace Mutare.GameCreator.Nakama
{
    public class NakamaManager : MonoBehaviour
    {
        public static Client client = null;
        public static ISession session = null;
        public static ISocket socket = null;
        public static IApiAccount account = null;

        public static UnityEvent OnSession_Connect = new UnityEvent();
        public static Dictionary<string, IChannel> openChannels = new Dictionary<string, IChannel>();
        [Header("Connection")]
        public string schema = "http";
        public string host = "172.0.0.1";
        public int port = 7350;
        public string serverkey = "defaultkey";

        [Space]
        [Header("Settings")]
        public bool connectOnStart = true;
        private void Awake()
        {
            if (connectOnStart)
            {
                NakamaConnect();
            }
        }
        public void NakamaConnect()
        {


            // OR
#if UNITY_WEBGL && !UNITY_EDITOR
        client = new Client(schema, host, port, serverkey, UnityWebRequestAdapter.Instance);
        ISocketAdapter adapter = new JsWebSocketAdapter();
#else
            ISocketAdapter adapter = new WebSocketAdapter();
            client = new Client(schema, host, port, serverkey);
#endif
            socket = Socket.From(client, adapter);
        }
        public static void Invoke_SessionConnect()
        {
            NakamaManager.OnSession_Connect.Invoke();
        }
    }
}

