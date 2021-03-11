using FayvitEventAgregator;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSender : NetworkBehaviour
{
    
    void Start()
    {
        var nId = GetComponent<NetworkIdentity>();

        if (nId.hasAuthority)
        {
            EventAgregator.AddListener(EventKey.networkSendRpcEvent, OnNetworkSendEvent);
            EventAgregator.AddListener(EventKey.requestServerEvent, OnRequestServerEvent);
        }
        }

    private void OnDestroy()
    {
        var nId = GetComponent<NetworkIdentity>();
        if (nId.hasAuthority)
        {
            EventAgregator.RemoveListener(EventKey.networkSendRpcEvent, OnNetworkSendEvent);
            EventAgregator.RemoveListener(EventKey.requestServerEvent, OnRequestServerEvent);
        }
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

    [Command]
    void CmdServerEvent(byte[] b)
    {
        PublishEventWithBytes(b);
    }

    [Command]
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
