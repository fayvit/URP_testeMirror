using UnityEngine;
using FayvitEventAgregator;
using System.Collections.Generic;
using Mirror;

public class MyConnectManager : NetworkManager//MonoBehaviourPunCallbacks
{
    private ConnectionState connState = ConnectionState.conectandoComoMaster;
    private TelepathyTransport transp;
    


    private enum ConnectionState
    { 
         conectandoComoMaster,
         conectandoParaJoin
    }

    // Start is called before the first frame update
    public override void Start()
    {
        transp = GetComponent<TelepathyTransport>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IpTextField(string content)
    {
        if (string.IsNullOrEmpty(content))
            networkAddress = "localhost";
        else
            networkAddress = content;
    }

    public void PortTextField(string content)
    {
        ushort u = ushort.Parse(content);
        transp.port = u;
    }

    public void BotaoCriarSala()
    {
        connState = ConnectionState.conectandoComoMaster;
        StartHost();
        EventAgregator.Publish(new GameEvent(EventKey.iniciandoConexao));
    }

    public void BotaoJuntarSe()
    {
        connState = ConnectionState.conectandoParaJoin;
        StartClient();
        EventAgregator.Publish(new GameEvent(EventKey.conectandoParaJoin));
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        #region suprimido
        //Debug.Log("\r\n Estou pegando minha conexão: " + NetworkServer.connections.Count + " isConnected " + NetworkClient.isConnected);
        //Debug.Log("\r\n" + conn);

        //Instantiate(myHeroPrefab, new Vector3(-2, 1, 1), Quaternion.identity);
        #endregion
        ClientScene.AddPlayer(conn);
        
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Vector3 pos = new Vector3(
                Random.Range(-9, 9), 1,
                Random.Range(-9, 9)
                );
        GameObject player = Instantiate(spawnPrefabs[0], pos, Quaternion.identity);

        if (conn != NetworkServer.localConnection)
            NetworkServer.SendToAll(new ChangePlayerNameMessage() { MySendObjects = { conn.connectionId } }); ;

        
            GameObject commSender = Instantiate(spawnPrefabs[2], pos, Quaternion.identity);
            NetworkServer.Spawn(commSender,conn);            
        

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    

    public override void OnStopClient()
    {
        EventAgregator.Publish(EventKey.stopClient);
    }

    #region Legacy

    //public override void OnStartServer()
    //{
    //    string s = "\r\n Chegou aqui: " + mode + ", networkadress: " + networkAddress + " isConnected " + NetworkClient.isConnected;
    //    EventAgregator.Publish(new GameEvent(EventKey.conexaoRealizada, s));


    //    base.OnStartServer();

    //}
    //public override void OnStartHost()
    //{
    //    if (connState == ConnectionState.conectandoComoMaster)
    //    {
    //        string s = System.Guid.NewGuid().ToString();

    //        EventAgregator.Publish(new GameEvent(EventKey.conexaoRealizada, s));

    //        //PhotonNetwork.CreateRoom(s);
    //    }
    //    else if (connState == ConnectionState.conectandoParaJoin)
    //    {
    //        base.OnStartHost();
    //        //PhotonNetwork.JoinLobby();
    //        EventAgregator.Publish(new GameEvent(EventKey.entrandoNoLobby));
    //    }
    //}
    //#region reference

    ////public override void OnClientConnect(NetworkConnection conn)
    ////{
    ////    uiTxt.text += "\r\n Estou pegando minha conexão: " + NetworkServer.connections.Count + " isConnected " + NetworkClient.isConnected;
    ////    uiTxt.text += "\r\n" + conn;


    ////    //Instantiate(myHeroPrefab, new Vector3(-2, 1, 1), Quaternion.identity);
    ////    ClientScene.AddPlayer(conn);

    ////    this.conn = conn;
    ////}

    ////public override void OnServerAddPlayer(NetworkConnection conn)
    ////{
    ////    GameObject player = Instantiate(myHeroPrefab.gameObject, new Vector3(-2, 1, 1), Quaternion.identity);

    ////    NetworkServer.AddPlayerForConnection(conn, player);
    ////}
    //#endregion

    //public override void OnServerConnect(NetworkConnection conn)
    //{
    //    Debug.Log("server connect");
    //}

    //public override void OnStartClient()
    //{

    //    if (connState == ConnectionState.conectandoComoMaster)
    //    {
    //        EventAgregator.Publish(new GameEvent(EventKey.salaCriada));
    //        //SupportSingleton.Instance.InvokeInRealTime(()=> { IniciarControle(); }, 1);
    //    }
    //    else if (connState == ConnectionState.conectandoParaJoin)
    //    {
    //        base.OnStartClient();
    //        EventAgregator.Publish(new GameEvent(EventKey.entrandoNoLobby));
    //    }
    //}

    //public override void OnClientConnect(NetworkConnection conn)
    //{
    //    if (connState == ConnectionState.conectandoComoMaster)
    //    {
    //        SupportSingleton.Instance.InvokeInRealTime(()=> { IniciarControle(conn); }, 1);
    //    }
    //    else
    //    {

    //        SupportSingleton.Instance.InvokeInRealTime(() => { IniciarControle(conn); Debug.Log("do cliente: "+conn); }, 1);
    //        EventAgregator.Publish(new GameEvent(EventKey.entrouNoLobby));
    //    }

    //    EventAgregator.Publish(new GameEvent(EventKey.desligarHudPhoton));

    //    //ClientScene.AddPlayer(conn);

    //    Debug.Log("\r\n Estou pegando minha conexão: " + NetworkServer.connections.Count + " isConnected " + NetworkClient.isConnected);
    //    Debug.Log("\r\n" + conn);


    //    //Instantiate(myHeroPrefab, new Vector3(-2, 1, 1), Quaternion.identity);
    //    ClientScene.AddPlayer(conn);

    //    Debug.Log("ativo: " + NetworkServer.active+" : "+NetworkClient.isConnected);
    //}

    //public override void OnClientDisconnect(NetworkConnection conn)
    //{
    //    Debug.Log("client desconnect");
    //    base.OnClientDisconnect(conn);
    //}

    //public override void OnClientError(NetworkConnection conn, int errorCode)
    //{
    //    Debug.Log("error");
    //    base.OnClientError(conn, errorCode);
    //}

    //public override void OnStartServer()
    //{
    //    Debug.Log("Servidor iniciado");
    //}

    //public override void OnStopClient()
    //{
    //    EventAgregator.Publish(EventKey.stopClient);
    //    base.OnStopClient();
    //}

    //public override void OnServerAddPlayer(NetworkConnection conn)
    //{

    //    Vector3 pos = new Vector3(
    //        Random.Range(-49, 49), 1,
    //        Random.Range(-49, 49)
    //        );

    //    GameObject G = Instantiate(spawnPrefabs[0], pos, Quaternion.identity);

    //    NetworkServer.AddPlayerForConnection(conn,G);

    //    CameraAplicator.cam.NewFocusForBasicCam(G.transform, 20, 20);

    //    Debug.Log("o que houve aqui");

    //}



    //void IniciarControle(NetworkConnection conn)
    //{
    //    //EventAgregator.Publish(new GameEvent(EventKey.desligarHudPhoton));

    //    ////ClientScene.AddPlayer(conn);

    //    //Debug.Log("\r\n Estou pegando minha conexão: " + NetworkServer.connections.Count + " isConnected " + NetworkClient.isConnected);
    //    //Debug.Log("\r\n" + conn);


    //    ////Instantiate(myHeroPrefab, new Vector3(-2, 1, 1), Quaternion.identity);
    //    //ClientScene.AddPlayer(conn);


    //}
    #endregion



}
