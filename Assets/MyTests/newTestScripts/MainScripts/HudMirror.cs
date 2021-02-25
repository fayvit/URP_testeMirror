using UnityEngine;
using UnityEngine.UI;
using FayvitEventAgregator;
using System;

public class HudMirror : MonoBehaviour
{
    [SerializeField] private Text mainText;
    [SerializeField] private GameObject botoesPrincipais;
    [SerializeField] private GameObject desligaveis;
    [SerializeField] private Text txtDoIPv4;
    [SerializeField] private Text txtDoIPv6;

    // Start is called before the first frame update
    void Start()
    {
        txtDoIPv4.text = "IP: "+IPManager.GetRealIP();
        txtDoIPv6.text = "Local IP: " + IPManager.GetIP(ADDRESSFAM.IPv4);
        EventAgregator.AddListener(EventKey.iniciandoConexao, OnStartConnect);
        EventAgregator.AddListener(EventKey.conexaoRealizada, OnConnect);
        EventAgregator.AddListener(EventKey.salaCriada, OnCreateRoom);
        EventAgregator.AddListener(EventKey.desligarHudPhoton, OnRequestOffHud);
        EventAgregator.AddListener(EventKey.conectandoParaJoin, OnStartConnectToJoin);
        EventAgregator.AddListener(EventKey.entrandoNoLobby, OnStartEnterLobby);
        EventAgregator.AddListener(EventKey.entrouNoLobby, OnEnterInLobby);
        EventAgregator.AddListener(EventKey.entrandoNaSala, OnEnterInTheRoom);
        EventAgregator.AddListener(EventKey.stopClient, OnStopClient);
    }

    private void OnDestroy()
    {
        EventAgregator.RemoveListener(EventKey.iniciandoConexao, OnStartConnect);
        EventAgregator.RemoveListener(EventKey.conexaoRealizada, OnConnect);
        EventAgregator.RemoveListener(EventKey.salaCriada, OnCreateRoom);
        EventAgregator.RemoveListener(EventKey.desligarHudPhoton, OnRequestOffHud);
        EventAgregator.RemoveListener(EventKey.conectandoParaJoin, OnStartConnectToJoin);
        EventAgregator.RemoveListener(EventKey.entrandoNoLobby, OnStartEnterLobby);
        EventAgregator.RemoveListener(EventKey.entrouNoLobby, OnEnterInLobby);
        EventAgregator.RemoveListener(EventKey.entrandoNaSala, OnEnterInTheRoom);
        EventAgregator.RemoveListener(EventKey.stopClient, OnStopClient);
    }

    private void OnStopClient(IGameEvent obj)
    {
        mainText.text = "A conexão falhou...";
        botoesPrincipais.SetActive(true);
    }

    private void OnEnterInTheRoom(IGameEvent obj)
    {
        string s = (string)obj.MySendObjects[0];
        mainText.text += "\n\r Entrando na sala: \n\r"+s;
    }

    private void OnEnterInLobby(IGameEvent obj)
    {
        mainText.text += "\n\r Entrou no Lobby, buscando salas";
    }

    private void OnStartEnterLobby(IGameEvent obj)
    {
        mainText.text += "\n\r Iniciando entrada no Lobby";
    }

    private void OnStartConnectToJoin(IGameEvent obj)
    {
        mainText.text = "Iniciando Conexão...";
        botoesPrincipais.SetActive(false);
    }

    private void OnRequestOffHud(IGameEvent obj)
    {
        desligaveis.SetActive(false);
    }

    private void OnCreateRoom(IGameEvent obj)
    {
        mainText.text += "\n\r Sala criada";
        
    }

    private void OnConnect(IGameEvent obj)
    {
        string s = (string)obj.MySendObjects[0];
        mainText.text += "\n\r Conexão realizada\n\r criando a sala: "+s;
    }

    private void OnStartConnect(IGameEvent obj)
    {
        mainText.text = "Iniciando Conexão...";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
