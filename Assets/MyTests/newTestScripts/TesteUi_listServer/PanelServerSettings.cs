using FayvitEventAgregator;
using Mirror;
using Mirror.Websocket;
using MyTestMirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelServerSettings : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.InputField doIp;
    [SerializeField] private UnityEngine.UI.InputField daPorta;
    private string serverName = "namelessServer";
    private string ipNumber = "";
    private TelepathyTransport tele;

    public System.Action OnServerAndPlayStart { get; set; }


    private void OnEnable()
    {
        tele = NetworkManager.singleton.GetComponent<TelepathyTransport>();
        doIp.text = IPManager.GetRealIP();
        daPorta.text = tele.port.ToString();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BtnVoltarAsOpcoes()
    {
        EventAgregator.PublishGameEvent(EventKey.backToMainMenu);
        gameObject.SetActive(false);
    }

    public void BtnOnlyServer()
    { 
    
    }

    public void BtnStartServerAndGame()
    {
        SingletonServerTick.Instance.StartServerProcedure(serverName,tele.port,ipNumber);
        gameObject.SetActive(false);
        OnServerAndPlayStart?.Invoke();
    }

    public void OnChangeNameServer(string s)
    {
        serverName = s;
    }

    public void OnChangePortNumber(string s)
    {
        int x;
        if (int.TryParse(s, out x))
        {
            tele.port = (ushort)x;
        }
    }

    public void OnChangeIpNumber(string s)
    {
        ipNumber = s;
    }
}
