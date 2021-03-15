using FayvitEventAgregator;
using FayvitSupportSingleton;
using Mirror;
using MyTestMirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSender : NetworkBehaviour
{
    void Start()
    {
        var nId = GetComponent<NetworkIdentity>();

        DontDestroyOnLoad(gameObject);
        EventAgregator.AddListener(EventKey.networkSendRpcEvent, OnNetworkSendEvent);
        EventAgregator.AddListener(EventKey.requestServerEvent, OnRequestServerEvent);
        EventAgregator.AddListener(EventKey.requestSendToOne, OnRequestSendToOne);
        EventAgregator.AddListener(EventKey.requestLogMessage, OnRequestLogMessage);
        NetworkClient.RegisterHandler<SendToOneMessage>(ReceiveForOne);


        SupportSingleton.Instance.InvokeOnEndFrame(() =>
        {
            PlayerSoulFromNetwork[] ps = FindObjectsOfType<PlayerSoulFromNetwork>();

            foreach (PlayerSoulFromNetwork soul in ps)
            {
                NetworkIdentity souIdentity = soul.GetComponent<NetworkIdentity>();
                if (souIdentity.hasAuthority)
                    EventAgregator.PublishGameEvent(EventKey.requestServerEvent, EventKey.changeCommandID, souIdentity.netId, netId);

            }
        });
        

        
    }

    private void OnDestroy()
    {
        var nId = GetComponent<NetworkIdentity>();

        EventAgregator.RemoveListener(EventKey.requestLogMessage, OnRequestLogMessage);
        EventAgregator.RemoveListener(EventKey.networkSendRpcEvent, OnNetworkSendEvent);
        EventAgregator.RemoveListener(EventKey.requestServerEvent, OnRequestServerEvent);
        EventAgregator.RemoveListener(EventKey.requestSendToOne, OnRequestSendToOne);
        NetworkClient.UnregisterHandler<SendToOneMessage>();

        if (nId.hasAuthority)
        {
            
        }
    }

    private void OnRequestLogMessage(IGameEvent obj)
    {
        string message = (string)obj.MySendObjects[0];

        Debug.Log(message);
    }

    private void ReceiveForOne(NetworkConnection arg1, SendToOneMessage arg2)
    {
        EventKey k = (EventKey)arg2.MySendObjects[0];
        arg2.MySendObjects.RemoveAt(0);

        EventAgregator.PublishGameEvent(arg2.MySendObjects.ToArray(),k);

    }

    private void OnRequestSendToOne(IGameEvent obj)
    {
        Debug.Log("Request Command Send To One");
        CmdSendToONe(ConvertEventObjectToByte(obj));
    }

    private void OnRequestServerEvent(IGameEvent obj)
    {
        CmdServerEvent(ConvertEventObjectToByte(obj));
    }

    byte[] ConvertEventObjectToByte(IGameEvent obj)
    {
        List<object> o = new List<object>();
        for (int i = 0; i < obj.MySendObjects.Length; i++)
            o.Add(obj.MySendObjects[i]);

        return  BytesToObject.ObjectForBytes(o);
    }

    void PublishEventWithBytes(byte[] b)
    {
        List<object> o = BytesToObject.ObjectWithBytes(b);
        EventKey k = (EventKey)o[0];
        o.RemoveAt(0);

        EventAgregator.PublishGameEvent(o.ToArray(), k);
    }

    private void OnNetworkSendEvent(IGameEvent obj)
    {
        CmdSendEvent(ConvertEventObjectToByte(obj));
    }

    [Command(ignoreAuthority = true)]
    void CmdSendToONe(byte[] b)
    {
        Debug.Log("Command Send To One");
        List<object> o = BytesToObject.ObjectWithBytes(b);
        NetworkIdentity nId = NetworkIdentity.spawned[(uint)o[0]];
        
        o.RemoveAt(0);

        NetworkServer.SendToClientOfPlayer(nId, new SendToOneMessage() { MySendObjects = o });
    }

    [Command(ignoreAuthority = true)]
    void CmdServerEvent(byte[] b)
    {
        PublishEventWithBytes(b);
    }

    [Command(ignoreAuthority = true)]
    void CmdSendEvent(byte[] b)
    {
        RpcSendEvent(b);
    }

    [ClientRpc]
    void RpcSendEvent(byte[] b)
    {
        PublishEventWithBytes(b);
    }
}
