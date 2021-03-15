using FayvitEventAgregator;
using FayvitSupportSingleton;
using FayvitUI;
using Mirror;
using MyTestMirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TesteCreationUIForListServer : MonoBehaviour
{
    [SerializeField] private PanelServerSettings pss;
    [SerializeField] private PanelPlayerList ppl;
    [SerializeField] private RoomListPanel rlp;

    private BasicMenu m;
    private List<PlayerDates> playerDates = new List<PlayerDates>();

    [System.Serializable]
    public struct PlayerDates
    {
        public uint netId;
        public int connectionID;
        public string playerName;
        public string latencia;
        public bool pronto;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        pss.OnServerAndPlayStart += () =>{
            rlp.StartHud();
        };

        EventAgregator.AddListener(EventKey.backToMainMenu,OnBackToMainMenu);
        EventAgregator.AddListener(EventKey.receiveServerTick, OnReceiveServerTick);
        EventAgregator.AddListener(EventKey.enterNewSoulPlayer, OnEnterNewSouPlayer);
        EventAgregator.AddListener(EventKey.sendPlayersDates, OnReceivePlayerDates);
        EventAgregator.AddListener(EventKey.stopClient, OnStopClient);
        EventAgregator.AddListener(EventKey.playerDisconnect, OnPlayerDisconnect);
        EventAgregator.AddListener(EventKey.updateRoomListInfos, OnUpdateRoomListInfo);
        EventAgregator.AddListener(EventKey.clickInEditPlayer, OnClickEditPlayerInRoomList);
        EventAgregator.AddListener(EventKey.clickInKickPlayer, OnClickIncKickPlayer);
        EventAgregator.AddListener(EventKey.clickPlayerReady, OnClickReadyInRoomList);
        //EventAgregator.AddListener(EventKey.serverRequestDisconnect, OnServerRequestDisconnect);
        EventAgregator.AddListener(EventKey.changeRoomInfoText, OnChangeRoomInfoText);
        SupportSingleton.Instance.InvokeOnEndFrame(() =>
        {
            m = SupportCreationUi.Instance.CreateBasicMenu(.25f, .45f, .75f, .95f).bMenu;
            m.StartHud(StarterMenu, new string[3] { "Abrir como servidor", "Juntar-se a um servidor", "Creditos" });
            //SingleMessagePanel s = SupportCreationUi.Instance.CreateSingleMessagePanel(.25f, .45f, .75f, .95f);
            //s.StartMessagePanel(() => { },"ABACABB","O But�o");
        });

    }

    private void OnDestroy()
    {
        EventAgregator.RemoveListener(EventKey.backToMainMenu, OnBackToMainMenu);
        EventAgregator.RemoveListener(EventKey.receiveServerTick, OnReceiveServerTick);
        EventAgregator.RemoveListener(EventKey.enterNewSoulPlayer, OnEnterNewSouPlayer);
        EventAgregator.RemoveListener(EventKey.sendPlayersDates, OnReceivePlayerDates);
        EventAgregator.RemoveListener(EventKey.stopClient, OnStopClient);
        EventAgregator.RemoveListener(EventKey.playerDisconnect, OnPlayerDisconnect);
        EventAgregator.RemoveListener(EventKey.updateRoomListInfos, OnUpdateRoomListInfo);
        EventAgregator.RemoveListener(EventKey.clickInEditPlayer, OnClickEditPlayerInRoomList);
        EventAgregator.RemoveListener(EventKey.clickInKickPlayer, OnClickIncKickPlayer);
        EventAgregator.RemoveListener(EventKey.clickPlayerReady, OnClickReadyInRoomList);
        //EventAgregator.RemoveListener(EventKey.serverRequestDisconnect, OnServerRequestDisconnect);
        EventAgregator.RemoveListener(EventKey.changeRoomInfoText, OnChangeRoomInfoText);
    }

    private void OnChangeRoomInfoText(IGameEvent obj)
    {

        string s = (string)obj.MySendObjects[0];
        rlp.ChangeInfoText(s);
    }

    //private void OnServerRequestDisconnect(IGameEvent obj)
    //{
    //    NetworkManager.singleton.StopClient();
    //}

    private void OnClickIncKickPlayer(IGameEvent obj)
    {
        
        Debug.Log("OnClickKick");
        int indice = (int)obj.MySendObjects[0];
        NetworkServer.connections[playerDates[indice].connectionID].Disconnect(); 
        //EventAgregator.PublishGameEvent(EventKey.requestSendToOne, playerDates[indice].netId, EventKey.serverRequestDisconnect);
    }

    void ChangeReady(bool ready,string s = "")
    {
        PlayerDates p = GetMyPlayerDates();
        PlayerSoulFromNetwork.GetMyPlayerSoul().MyPlayerDate = p;
        p.pronto = ready;
        if (!string.IsNullOrEmpty(s))
        {
            p.playerName = s;
        }
        EventAgregator.PublishGameEvent(EventKey.requestServerEvent, EventKey.updateRoomListInfos, p);
    }

    private void OnClickEditPlayerInRoomList(IGameEvent obj)
    {
        ChangeReady(false);
    }

    private void OnClickReadyInRoomList(IGameEvent obj)
    {
        string nome = (string)obj.MySendObjects[0];
        ChangeReady(true,nome);
    }

    private void OnUpdateRoomListInfo(IGameEvent obj)
    {
        PlayerDates P = (PlayerDates)obj.MySendObjects[0];
        playerDates[GetIndexOfPlayerDatesWithId(P.netId)] = P;
        EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.sendPlayersDates, playerDates);
    }

    private void OnPlayerDisconnect(IGameEvent obj)
    {
        int connID = (int)obj.MySendObjects[0];
        int index = -1;
        for (int i = 0; i < playerDates.Count; i++)
        {
            if (playerDates[i].connectionID == connID)
                index = i;
        }

        if (index > -1)
            playerDates.RemoveAt(index);

        EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.sendPlayersDates, playerDates);
    }

    private void OnStopClient(IGameEvent obj)
    {
        if(!NetworkClient.active)
            playerDates = new List<PlayerDates>();

        if (rlp.IsActive)
        {
            rlp.FinishHud();
            SingleMessagePanel s = SupportCreationUi.Instance.CreateSingleMessagePanel(.25f, .45f, .75f, .75f);
            s.StartMessagePanel(BackToMainMenu, "A conex�o foi perdida ou o servidor cancelou a partida. Voltando para o menu inicial", "Ok");
        }

    }

    private void OnReceivePlayerDates(IGameEvent obj)
    {
        List<PlayerDates> L = (List<PlayerDates>)obj.MySendObjects[0];
        playerDates = L;
        rlp.RestartHud(L);
    }

    private void OnEnterNewSouPlayer(IGameEvent obj)
    {
        playerDates.Add(new PlayerDates()
        {
            netId = (uint)obj.MySendObjects[0],
            latencia = ((float)obj.MySendObjects[1]).ToString(),
            playerName = "nameless: " + (int)obj.MySendObjects[2],
            pronto = false,
            connectionID = (int)obj.MySendObjects[2]
        });

        EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.sendPlayersDates, playerDates);
    }

    private void OnReceiveServerTick(IGameEvent obj)
    {
        Dictionary<string, ServerStatus> list = (Dictionary<string, ServerStatus>)obj.MySendObjects[0];
        ppl.ReceiveServerTick(list,SelectJoin);
    }

    void OnBackToMainMenu(IGameEvent e)
    {
        m.StartHud(StarterMenu, new string[3] { "Abrir como servidor", "Juntar-se a um servidor", "Creditos" });
    }

    void StarterMenu(int x)
    {
        switch (x)
        {
            case 0:
                m.FinishHud();
                pss.gameObject.SetActive(true);
                //gameObject.SetActive(false);
            break;
            case 1:
                m.FinishHud();
                SingletonClientTick.Instance.StartServerProcedure();
                ppl.StartHud(0,SelectJoin);
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectJoin(int x)
    {
        ServerStatus server = ppl.ListActive.Values.ToList()[x];
        NetworkManager.singleton.networkAddress = server.ip;
        NetworkManager.singleton.GetComponent<TelepathyTransport>().port = server.port;
        NetworkManager.singleton.StartClient();
        //NetworkManager.singleton.OnClientConnect+=
        SingletonClientTick.Instance.Finish();
        ppl.FinishHud();
        rlp.StartHud();
    }

    public void BackToMainMenu()
    {
        OnBackToMainMenu(null);
    }

    public void BackToMainMenuInListPanel()
    {
        ppl.FinishHud();
        rlp.FinishHud();
        if (NetworkServer.active)
        {
            SingletonServerTick.Instance.FinishServerTick();
            NetworkManager.singleton.StopHost();
        }
        else
            NetworkManager.singleton.StopClient();

        OnBackToMainMenu(null);

        playerDates = new List<PlayerDates>();

    }

    public void BackToMainMenuInListServers()
    {
        ppl.FinishHud();
        SingletonClientTick.Instance.Finish();
        OnBackToMainMenu(null);
    }

    public void BtnIniciarJogo()
    {
        rlp.IniciarEstadoDeInicioDeJogo();
    }

    int GetIndexOfPlayerDatesWithId(uint netId)
    {
        int retorno = -1;
        for (int i = 0; i < playerDates.Count; i++)
        {
            if (playerDates[i].netId == netId)
            {
                retorno = i;
            }
        }

        return retorno;
    }

    PlayerDates GetMyPlayerDates()
    {
        PlayerDates p = new PlayerDates();
        for (int i = 0; i < playerDates.Count; i++)
        {
            if (NetworkIdentity.spawned[playerDates[i].netId].isLocalPlayer)
                p = playerDates[i];
        }

        return p;
    }
}
