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
            EventAgregator.AddListener(EventKey.networkSendEvent,OnNetworkSendEvent);
    }

    private void OnDestroy()
    {
        var nId = GetComponent<NetworkIdentity>();
        if (nId.hasAuthority)
            EventAgregator.RemoveListener(EventKey.networkSendEvent, OnNetworkSendEvent);
    }

    private void OnNetworkSendEvent(IGameEvent obj)
    {
        List<object> o = new List<object>();
        for (int i = 0; i < obj.MySendObjects.Length; i++)
            o.Add(obj.MySendObjects[i]);

        byte[] b = BytesToObject.ObjectForBytes(o);

        CmdSendEvent(b);
    }

    [Command]
    void CmdSendEvent(byte[] b)
    {
        RpcSendEvent(b);
    }

    [ClientRpc]
    void RpcSendEvent(byte[] b)
    {
        List<object> o = BytesToObject.ObjectWithBytes(b);
        EventKey k = (EventKey)o[0];
        o.RemoveAt(0);

        EventAgregator.PublishGameEvent(o.ToArray(),k);
    }
}
