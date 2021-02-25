using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.IO;
using System.Text;
using System.Net;
using Mirror.Examples.ListServer;
using System;

public class ListServerFacade : MonoBehaviour
{
    public event Action<ConnectState,Dictionary<string, ServerStatus>> receivedTickResult;
    public event Action<Dictionary<string, ServerStatus>> clientConnected;
    public event Action desconnectedClient;
    public event Action failToConnect;
    public event Action createServer;
    public event Action failOnCreateServer;

    private Telepathy.Client gameServerToListenConnection = new Telepathy.Client();
    private Telepathy.Client clientToListenConnection = new Telepathy.Client();
    private ListServerConnection serversConn;
    private Dictionary<string, ServerStatus> list = new Dictionary<string, ServerStatus>();

    private static ListServerFacade instance;

    bool IsConnecting => NetworkClient.active && !ClientScene.ready;
    bool FullyConnected => NetworkClient.active && ClientScene.ready;
    bool UseGameServerToListen => NetworkServer.active;
    bool UseClientToListen => !NetworkManager.isHeadless && !NetworkServer.active && !FullyConnected;

    public static ListServerFacade Instance { 
        get {
            if (instance == null)
            {
                GameObject G = new GameObject();
                G.name = "List Server Facade";
                instance = G.AddComponent<ListServerFacade>();
            }
            
            return instance;
        } 
    }


    public void StartTick(ListServerConnection serversConn)
    {
        this.serversConn = serversConn;
       InvokeRepeating(nameof(Tick), 0, 1);
    }

    public void CancelTick()
    {
        CancelInvoke();
    }

    void Tick()
    {
        TickGameServer();
        TickClient();
    }

    void TickClient()
    {
        // receive client data from listen
        if (UseClientToListen)
        {
            // receive latest game server info
            while (clientToListenConnection.GetNextMessage(out Telepathy.Message message))
            {
                // connected?
                if (message.eventType == Telepathy.EventType.Connected)
                {
                    clientConnected?.Invoke(list);
                    Debug.Log("[List Server] Client connected!");
                }
                // data message?
                else if (message.eventType == Telepathy.EventType.Data)
                    ParseMessage(message.data);
                // disconnected?
                else if (message.eventType == Telepathy.EventType.Disconnected)
                {
                    Debug.Log("[List Server] Client disconnected.");
                    desconnectedClient?.Invoke();
                }

                else if (message.eventType == Telepathy.EventType.FailToConnect)
                {
                    failToConnect?.Invoke();
                    Debug.Log("[List Server] Client fail to connect.");
                }

            }

            // connected yet?
            if (clientToListenConnection.Connected)
            {
                

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
                Debug.Log("[List Server] Client connecting...");
                clientToListenConnection.Connect(serversConn.listServerIp, serversConn.clientToListenPort);
            }
        }
        // shouldn't use client, but still using it? (e.g. after joining)
        else if (clientToListenConnection.Connected)
        {
            clientToListenConnection.Disconnect();
            list.Clear();
        }

        // refresh UI afterwards
        TestTickResult();
    }

    void ParseMessage(byte[] bytes)
    {
        // note: we don't use ReadString here because the list server
        //       doesn't know C#'s '7-bit-length + utf8' encoding for strings
        BinaryReader reader = new BinaryReader(new MemoryStream(bytes, false), Encoding.UTF8);
        byte ipBytesLength = reader.ReadByte();
        byte[] ipBytes = reader.ReadBytes(ipBytesLength);
        string ip = new IPAddress(ipBytes).ToString();
        //ushort port = reader.ReadUInt16(); <- not all Transports use a port. assume default.
        ushort players = reader.ReadUInt16();
        ushort capacity = reader.ReadUInt16();
        ushort titleLength = reader.ReadUInt16();
        string title = Encoding.UTF8.GetString(reader.ReadBytes(titleLength));
        //logger.Log("PARSED: ip=" + ip + /*" port=" + port +*/ " players=" + players + " capacity= " + capacity + " title=" + title);

        // build key
        string key = ip/* + ":" + port*/;

        // find existing or create new one
        if (list.TryGetValue(key, out ServerStatus server))
        {
            // refresh
            server.title = title;
            server.players = players;
            server.capacity = capacity;
        }
        else
        {
            // create
            server = new ServerStatus(ip, /*port,*/ title, players, capacity);
        }

        // save
        list[key] = server;
    }

    void TickGameServer()
    {
        // send server data to listen
        if (UseGameServerToListen)
        {
            while (gameServerToListenConnection.GetNextMessage(out Telepathy.Message message))
            {
                // connected?
                if (message.eventType == Telepathy.EventType.Connected)
                {
                    //clientConnected?.Invoke(list);
                    createServer?.Invoke();
                    Debug.Log("[Server]  connected!");
                }
                else if (message.eventType == Telepathy.EventType.Disconnected)
                {
                    Debug.Log("[Server] disconnected.");
                    //desconnectedClient?.Invoke();
                }

                else if (message.eventType == Telepathy.EventType.FailToConnect)
                {
                    CancelTick();
                    failOnCreateServer?.Invoke();
                    Debug.Log("[Server] Server fail to create.");
                }

            }
            // connected yet?
            if (gameServerToListenConnection.Connected)
            {
                SendStatus();
            }
            // otherwise try to connect
            // (we may have just started the game)
            else if (!gameServerToListenConnection.Connecting)
            {
                Debug.Log("[List Server] GameServer connecting......");
                gameServerToListenConnection.Connect(serversConn.listServerIp, serversConn.gameServerToListenPort);
            }
        }
        // shouldn't use game server, but still using it?
        else if (gameServerToListenConnection.Connected)
        {
            gameServerToListenConnection.Disconnect();
        }
    }

    void TestTickResult()
    {
        ConnectState state = ConnectState.None;
        // only show while client not connected and server not started
        if (!NetworkManager.singleton.isNetworkActive || IsConnecting)
        {
            //mainPanel.SetActive(true);

            // status text
            if (clientToListenConnection.Connecting)
            {
                //statusText.color = Color.yellow;
                // statusText.text = "Connecting...";
                //Debug.Log("[Facade: connectando]");
                state = ConnectState.Connecting;
            }
            else if (clientToListenConnection.Connected)
            {
                //statusText.color = Color.green;
                //statusText.text = "Connected!";
                //Debug.Log("[Facade: conectado!!]");
                state = ConnectState.Connected;
            }
            else
            {
                //statusText.color = Color.gray;
                //Debug.Log("[Facade: Desconectado]");
                state = ConnectState.Disconnected;
            }

            // instantiate/destroy enough slots
            //BalancePrefabs(slotPrefab.gameObject, list.Count, content);

            receivedTickResult?.Invoke(state,list);
        }
    }

    void SendStatus()
    {
        BinaryWriter writer = new BinaryWriter(new MemoryStream());

        // create message
        writer.Write((ushort)NetworkServer.connections.Count);
        writer.Write((ushort)NetworkManager.singleton.maxConnections);
        byte[] titleBytes = Encoding.UTF8.GetBytes(serversConn.gameServerTitle);
        writer.Write((ushort)titleBytes.Length);
        writer.Write(titleBytes);
        writer.Flush();

        // list server only allows up to 128 bytes per message
        if (writer.BaseStream.Position <= 128)
        {
            // send it
            gameServerToListenConnection.Send(((MemoryStream)writer.BaseStream).ToArray());
        }
        else Debug.LogError("[List Server] List Server will reject messages longer than 128 bytes. Please use a shorter title.");
    }

    void OnApplicationQuit()
    {
        if (gameServerToListenConnection.Connected)
            gameServerToListenConnection.Disconnect();
        if (clientToListenConnection.Connected)
            clientToListenConnection.Disconnect();
    }


}
