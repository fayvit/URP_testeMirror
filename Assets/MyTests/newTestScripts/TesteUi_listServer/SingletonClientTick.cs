using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.IO;
using System.Text;
using System.Net;
using System.Linq;
using FayvitEventAgregator;

namespace MyTestMirror
{
    public class SingletonClientTick : MonoBehaviour
    {
        private string listServerIp = "35.243.149.104";
        private int clientToListenPort = 8888;
        private Telepathy.Client clientToListenConnection = new Telepathy.Client();
        private static readonly ILogger logger = LogFactory.GetLogger(typeof(SingletonClientTick));
        private Dictionary<string, ServerStatus> list = new Dictionary<string, ServerStatus>();


        private static SingletonClientTick instance;

        public static SingletonClientTick Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject G = new GameObject();
                    G.name = "Fayvit_SingletonClientTick";
                    DontDestroyOnLoad(G);

                    instance = G.AddComponent<SingletonClientTick>();
                }

                return instance;

            }
        }

        public void Finish()
        {
            OnApplicationQuit();

            Destroy(gameObject);
        }

        // Use this for initialization
        void Start()
        {
            SingletonClientTick[] g = FindObjectsOfType<SingletonClientTick>();

            if (g.Length > 1)
                Destroy(gameObject);
            else
                instance = this;
        }

        public void StartServerProcedure()
        {
            InvokeRepeating(nameof(Tick), 0, 1);
        }

        void Tick()
        {
            TickClient();
        }

        bool FullyConnected() => NetworkClient.active && ClientScene.ready;

        // should we use the client to listen connection?
        bool UseClientToListen()
        {
            return !NetworkManager.isHeadless && !NetworkServer.active && !FullyConnected();
        }

        void TickClient()
        {
            // receive client data from listen
            if (UseClientToListen())
            {
                // connected yet?
                if (clientToListenConnection.Connected)
                {
                    // receive latest game server info
                    while (clientToListenConnection.GetNextMessage(out Telepathy.Message message))
                    {
                        
                        // connected?
                        if (message.eventType == Telepathy.EventType.Connected)
                            logger.Log("[List Server] Client connected!");
                        // data message?
                        else if (message.eventType == Telepathy.EventType.Data)
                            ParseMessage(message.data);
                        // disconnected?
                        else if (message.eventType == Telepathy.EventType.Disconnected)
                            logger.Log("[List Server] Client disconnected.");
                    }

#if !UNITY_WEBGL
                    // Ping isn't known in WebGL builds
                    // ping again if previous ping finished
                    foreach (ServerStatus server in list.Values)
                    {
                        if (server.ping.isDone)
                        {
                            server.lastLatency = server.ping.time;
                            server.ping = new Ping(server.ip);
                        }
                    }
#endif
                }
                // otherwise try to connect
                // (we may have just joined the menu/disconnect from game server)
                else if (!clientToListenConnection.Connecting)
                {
                    logger.Log("[List Server] Client connecting...");
                    clientToListenConnection.Connect(listServerIp, clientToListenPort);
                }
            }
            // shouldn't use client, but still using it? (e.g. after joining)
            else if (clientToListenConnection.Connected)
            {
                clientToListenConnection.Disconnect();
                list.Clear();
            }

            // refresh UI afterwards
            OnUI();
        }

        void OnUI()
        {
            
            EventAgregator.PublishGameEvent(EventKey.receiveServerTick,list);

            for (int i = 0; i < list.Count; i++)
            {
                ServerStatus server = list.Values.ToList()[i];
            }
        }

        void ParseMessage(byte[] bytes)
        {
            // note: we don't use ReadString here because the list server
            //       doesn't know C#'s '7-bit-length + utf8' encoding for strings
            BinaryReader reader = new BinaryReader(new MemoryStream(bytes, false), Encoding.UTF8);
            byte ipBytesLength = reader.ReadByte();
            byte[] ipBytes = reader.ReadBytes(ipBytesLength);
            string ip = new IPAddress(ipBytes).ToString();
            //ushort port = reader.ReadUInt16(); //<- not all Transports use a port. assume default.
            ushort players = reader.ReadUInt16();
            ushort capacity = reader.ReadUInt16();
            //ushort port = reader.ReadUInt16();
            ushort titleLength = reader.ReadUInt16();
            string title = Encoding.UTF8.GetString(reader.ReadBytes(titleLength));

            string[] tent = title.Split( new string[1] { SingletonServerTick.guidCodeForSplit },System.StringSplitOptions.None);

            title = tent[0];
            ushort port;

            if (tent.Length > 1)
            {
                if (!ushort.TryParse(tent[1], out port))
                {
                    Debug.Log("Algo errado");

                    port = NetworkManager.singleton.GetComponent<TelepathyTransport>().port;
                }
                else
                {
                    Debug.Log("A porta recebida pela mensagem: " + port);
                }

                //logger.Log("PARSED: ip=" + ip + /*" port=" + port +*/ " players=" + players + " capacity= " + capacity + " title=" + title);

                // build key


                string key = ip + " : " + title/* + ":" + port*/;

                // find existing or create new one
                if (list.TryGetValue(key, out ServerStatus server))
                {
                    // refresh
                    server.title = title;
                    server.players = players;
                    server.capacity = capacity;
                    server.port = port;
                }
                else
                {
                    // create
                    server = new ServerStatus(ip, port, title, players, capacity);
                }

                // save
                list[key] = server;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnApplicationQuit()
        {
            //if (gameServerToListenConnection.Connected)
            //    gameServerToListenConnection.Disconnect();
            if (clientToListenConnection.Connected)
                clientToListenConnection.Disconnect();
        }
    }
}