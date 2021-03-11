using FayvitEventAgregator;
using FayvitSupportSingleton;
using Mirror;
using Mirror.Websocket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTestMirror
{
    public class PlayerSoulFromNetwork : NetworkBehaviour
    {
        private NetworkIdentity nId;
        [SyncVar] private int connId;
        
        public int ConnectionID { get=>connId; set=>connId=value; }
        // Start is called before the first frame update
        void Start()
        {
            nId = GetComponent<NetworkIdentity>();

            if (nId.isLocalPlayer)
            {
                SupportSingleton.Instance.InvokeOnEndFrame(gameObject,() =>
                {
                    EventAgregator.PublishGameEvent(
                        EventKey.requestServerEvent,
                        EventKey.enterNewSoulPlayer,
                        netId,
                        NetworkClient.connection.lastMessageTime,
                        ConnectionID
                        );
                });
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
