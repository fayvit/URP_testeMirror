using FayvitEventAgregator;
using FayvitSupportSingleton;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTestMirror
{
    public class PlayerSoulFromNetwork : NetworkBehaviour
    {
        private NetworkIdentity nId;
        [SyncVar] private int connId;
        [SyncVar] private uint commandSenderNetID;

        public TesteCreationUIForListServer.PlayerDates MyPlayerDate { get; set; }
        
        public int ConnectionID { get=>connId; set=>connId=value; }
        public uint CommandSenderNetID { get => commandSenderNetID; set => commandSenderNetID = value; }

        public static PlayerSoulFromNetwork GetMyPlayerSoul()
        {
            PlayerSoulFromNetwork conn = null;

            PlayerSoulFromNetwork[] souls = FindObjectsOfType<PlayerSoulFromNetwork>();

            foreach (var soul in souls)
            {
                if (NetworkIdentity.spawned[soul.netId].isLocalPlayer)
                    conn = soul;
                
            }

            return conn;
        }

        public static PlayerSoulFromNetwork GetPlayerSoulByID(int connID)
        {
            PlayerSoulFromNetwork conn = null;

            PlayerSoulFromNetwork[] souls = FindObjectsOfType<PlayerSoulFromNetwork>();

            foreach (var soul in souls)
            {
                if (soul.ConnectionID == connID)
                    conn = soul;

            }

            return conn;
        }
        // Start is called before the first frame update
        void Start()
        {
            
            
            nId = GetComponent<NetworkIdentity>();

            DontDestroyOnLoad(gameObject);
            

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
            
            if (NetworkServer.active)
                EventAgregator.AddListener(EventKey.changeCommandID, OnChangeCommandID);
        }

        private void OnDestroy()
        {
            EventAgregator.RemoveListener(EventKey.changeCommandID, OnChangeCommandID);
        }

        private void OnChangeCommandID(IGameEvent obj)
        {
            if ((uint)obj.MySendObjects[0] == netId)
                commandSenderNetID = (uint)obj.MySendObjects[1];
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
