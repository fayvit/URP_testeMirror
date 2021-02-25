using Mirror;
using Mirror.Examples.ListServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ListServerFrontEnd : MonoBehaviour
{
    [Header("List Server Connection")]
    [SerializeField] private ListServerConnection serverData = default;

    private Dictionary<string, ServerStatus> listaDeSalas=new Dictionary<string, ServerStatus>();

    /*
    public string listServerIp = "35.243.149.104";
    public ushort gameServerToListenPort = 8887;
    public ushort clientToListenPort = 8888;
    public string gameServerTitle = "Deathmatch";*/

    [Header("UI")]
    [SerializeField] private Text grandeTexto;
    public static ListServerFrontEnd instance;

    private void Start()
    {
        grandeTexto.text = "Start Tick";
        instance = this;
        ListServerFacade L = ListServerFacade.Instance;
        L.StartTick(serverData);
        L.clientConnected += OnClientConnected;
        L.receivedTickResult += OnReceivedTickResult;
    }

    public void OnClientConnect(NetworkConnection conn)
    {
        grandeTexto.text += "\n\r Conectou-se: "+conn.address+":"+conn.connectionId;
    }

    private void OnReceivedTickResult(ConnectState arg1, Dictionary<string, ServerStatus> arg2)
    {
       // if (listaDeSalas != arg2)
        {
            listaDeSalas = arg2;
            if (arg2.Count > 0)
                foreach (string key in arg2.Keys)
                {
                    ServerStatus S = arg2[key];
                    Debug.Log(S.ip + " : " + S.players + " : " + S.title + " : " + S.ping);
                }
            else
                Debug.Log("sem elementos na lista");
        }
    }

    private void OnClientConnected(Dictionary<string, ServerStatus> obj)
    {
        Debug.Log("Estou Conectado");
    }

    public void JoinInTheHost()
    {

        string ip = listaDeSalas.Keys.ToList()[0];

        ListServerFacade.Instance.CancelTick();
        /*
        if (ip != IPManager.GetRealIP())
        {
            Debug.Log("ip diferente");
            NetworkManager.singleton.networkAddress = ip;
        }
        else
        {
            Debug.Log("Estou usando localhost");
            NetworkManager.singleton.networkAddress = "localhost";
        }*/

        NetworkManager.singleton.networkAddress = ip;
        
        
        NetworkManager.singleton.StartClient();
    }

    public void OpenHost()
    {
        NetworkManager.singleton.StartHost();
    }

    public void OpenServer()
    {
        NetworkManager.singleton.StartServer();
    }
}

[System.Serializable]
public struct ListServerConnection
{
    public string listServerIp;
    public ushort gameServerToListenPort;
    public ushort clientToListenPort;
    public string gameServerTitle;
}
