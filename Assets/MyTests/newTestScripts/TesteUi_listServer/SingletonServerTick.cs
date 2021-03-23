using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.IO;
using System.Text;
using System.Net;
using System;

namespace MyTestMirror
{

    public class ServerStatus:ICloneable
    {
        public string ip;
        // not all transports use a port. assume default port. feel free to also send a port if needed.
        public ushort port;
        public string title;
        public ushort players;
        public ushort capacity;

        public int lastLatency = -1;
#if !UNITY_WEBGL
        // Ping isn't known in WebGL builds
        public Ping ping;
#endif
        public ServerStatus(string ip, ushort port, string title, ushort players, ushort capacity)
        {
            this.ip = ip;
            this.port = port;
            this.title = title;
            this.players = players;
            this.capacity = capacity;
#if !UNITY_WEBGL
            // Ping isn't known in WebGL builds
            ping = new Ping(ip);
#endif
        }

        public object Clone()
        {
            return new ServerStatus(ip, port, title, players, capacity);
        }
    }
    public class SingletonServerTick : MonoBehaviour
    {
        public static string guidCodeForSplit = "4dd2aee2-6179-4109-9622-a9fea86cfc8d";
        private string gameServerTitle = "";
        private string listServerIp = "35.243.149.104";
        private int gameServerToListenPort = 8887;
        private Telepathy.Client gameServerToListenConnection = new Telepathy.Client();
        private Dictionary<string, ServerStatus> list = new Dictionary<string, ServerStatus>();


        private static readonly ILogger logger = LogFactory.GetLogger(typeof(SingletonServerTick));

        private static SingletonServerTick instance;        

        public static SingletonServerTick Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject G = new GameObject();
                    G.name = "Fayvit_SingletonServerTick";
                    DontDestroyOnLoad(G);

                    instance = G.AddComponent<SingletonServerTick>();
                }

                return instance;

            }
        }
        // Use this for initialization
        void Start()
        {
            SingletonServerTick[] g = FindObjectsOfType<SingletonServerTick>();

            if (g.Length > 1)
                Destroy(gameObject);
            else
                instance = this;
        }

        public void StartServerProcedure(string nameServer, int porta, string ip)
        {
            gameServerTitle = nameServer;
            NetworkManager.singleton.StartHost();
            InvokeRepeating(nameof(Tick), 0, 1);
        }

        void Tick()
        {
            TickGameServer();
        }

        bool UseGameServerToListen()
        {
            return NetworkServer.active;
        }

        void SendStatus()
        {
         
            BinaryWriter writer = new BinaryWriter(new MemoryStream());
            ushort portNumber = NetworkManager.singleton.GetComponent<TelepathyTransport>().port;

            string serverNameAndPort = gameServerTitle + guidCodeForSplit + portNumber;
            // create message
            writer.Write((ushort)NetworkServer.connections.Count);
            writer.Write((ushort)NetworkManager.singleton.maxConnections);
            //writer.Write(portNumber);
            byte[] titleBytes = Encoding.UTF8.GetBytes(serverNameAndPort);
            writer.Write((ushort)titleBytes.Length);
            writer.Write(titleBytes);
            writer.Flush();

            Debug.Log("enviando");
            // list server only allows up to 128 bytes per message
            if (writer.BaseStream.Position <= 128)
            {
                
                // send it
                gameServerToListenConnection.Send(((MemoryStream)writer.BaseStream).ToArray());
            }
            else logger.LogError("[List Server] List Server will reject messages longer than 128 bytes. Please use a shorter title.");
        }

        void TickGameServer()
        {
            // send server data to listen
            if (UseGameServerToListen())
            {
                // connected yet?
                if (gameServerToListenConnection.Connected)
                {
                    SendStatus();
                }
                // otherwise try to connect
                // (we may have just started the game)
                else if (!gameServerToListenConnection.Connecting)
                {
                    logger.Log("[List Server] GameServer connecting......");
                    gameServerToListenConnection.Connect(listServerIp, gameServerToListenPort);
                }
            }
            // shouldn't use game server, but still using it?
            else if (gameServerToListenConnection.Connected)
            {
                gameServerToListenConnection.Disconnect();
            }
        }

        void Update()
        {

        }

        void OnApplicationQuit()
        {
            if (gameServerToListenConnection.Connected)
                gameServerToListenConnection.Disconnect();
            //if (clientToListenConnection.Connected)
            //    clientToListenConnection.Disconnect();
        }

        public void FinishServerTick()
        {
            OnApplicationQuit();
            CancelInvoke();
            Destroy(gameObject);
        }
    }
}