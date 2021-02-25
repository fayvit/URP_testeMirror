using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.UI;

public class FirstTestScript : NetworkManager
{
    [SerializeField] private InputField uiTxt = default;
    [SerializeField] private NetworkIdentity myHeroPrefab = default;
    NetworkConnection conn;

    public override void OnClientConnect(NetworkConnection conn)
    {
        uiTxt.text += "\r\n Estou pegando minha conexão: " + NetworkServer.connections.Count + " isConnected " + NetworkClient.isConnected;
        uiTxt.text += "\r\n" + conn;
        

        //Instantiate(myHeroPrefab, new Vector3(-2, 1, 1), Quaternion.identity);
        ClientScene.AddPlayer(conn);
        
        this.conn = conn;
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(myHeroPrefab.gameObject, new Vector3(-2, 1, 1), Quaternion.identity);

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        /*
        Debug.Log(mode+" : "+ IPManager.GetIP(ADDRESSFAM.IPv4)+" : "+ IPManager.GetIP(ADDRESSFAM.IPv6));

        networkAddress = IPManager.GetIP(ADDRESSFAM.IPv4);
        Invoke("Proximo", 2);*/
    }


    public override void OnStartServer()
    {
        uiTxt.text += "\r\n Chegou aqui: " + mode + ", networkadress: " + networkAddress+" isConnected "+NetworkClient.isConnected;

        base.OnStartServer();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnGUI()
    {
        if (GUI.Button(ScreenRect(0.01f, 0.01f, 0.25f, 0.1f), "Start Host"))
        {
            StartHost();
        }else if (GUI.Button(ScreenRect(0.26f, 0.01f, 0.25f, 0.1f), "Start Client"))
        {
            StartClient();
        }
    }

    private Rect ScreenRect(float percentStartX, float percentStartY, float percenteLengthX, float percentLengthY)
    {
        int w = Screen.width;
        int h = Screen.height;

        return new Rect(percentStartX*w,percentStartY*h,percenteLengthX*w,percentLengthY*h);
    }

}

public class IPManager
{
    public static string GetRealIP()
    {
        string address = "";
        WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
        using (WebResponse response = request.GetResponse())
        using (System.IO.StreamReader stream = new System.IO.StreamReader(response.GetResponseStream()))
        {
            address = stream.ReadToEnd();
        }

        int first = address.IndexOf("Address: ") + 9;
        int last = address.LastIndexOf("</body>");
        address = address.Substring(first, last - first);

        return address;
    }

    public static string GetIP(ADDRESSFAM Addfam)
    {
        //Return null if ADDRESSFAM is Ipv6 but Os does not support it
        if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6)
        {
            return null;
        }

        string output = "";

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

            if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif 
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    //IPv4
                    if (Addfam == ADDRESSFAM.IPv4)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }

                    //IPv6
                    else if (Addfam == ADDRESSFAM.IPv6)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
        }
        return output;
    }
}

public enum ADDRESSFAM
{
    IPv4, IPv6
}
