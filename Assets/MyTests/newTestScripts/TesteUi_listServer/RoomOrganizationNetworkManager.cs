using FayvitEventAgregator;
using FayvitSupportSingleton;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTestMirror
{
    public class RoomOrganizationNetworkManager : NetworkManager
    {

        public override void OnServerReady(NetworkConnection conn)
        {
            Debug.Log("OnServerReady");
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            Debug.Log("[load scene in client ] OnClientSceneChanged");
        }

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            EventAgregator.PublishGameEvent(EventKey.requestServerEvent, EventKey.clientSceneLoadReady, newSceneName,
                PlayerSoulFromNetwork.GetMyPlayerSoul().netId,sceneOperation
                );

            Debug.Log("[start load in client] OnClientChangeScene");
        }

        public override void OnServerChangeScene(string newSceneName)
        {
            Debug.Log("OnServerChangeScene");
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            Debug.Log("OnServerSceneChanged");
        }

        public override void ServerChangeScene(string newSceneName)
        {
            Debug.Log("ServerChangeScene");
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            ClientScene.AddPlayer(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            
            GameObject player = Instantiate(spawnPrefabs[0]);

            player.GetComponent<PlayerSoulFromNetwork>().ConnectionID = conn.connectionId;
            NetworkServer.AddPlayerForConnection(conn, player);

            if (conn.connectionId == 0)
            {
                GameObject commSender = Instantiate(spawnPrefabs[1]);
                NetworkServer.Spawn(commSender, conn);
            }
        }

        public override void OnStopClient()
        {
            Debug.Log("Stop Client");
            EventAgregator.PublishGameEvent(EventKey.stopClient);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("OnClient disconnect");
            base.OnClientDisconnect(conn);
        }

        bool serverDesconectando = false;

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            
            Debug.Log("disconnect?: "+ NetworkClient.connection+" : "+conn+" : "+ NetworkConnection.LocalConnectionId);

            if (conn.connectionId != NetworkClient.connection.connectionId && !serverDesconectando)
                EventAgregator.PublishGameEvent(EventKey.requestServerEvent, EventKey.playerDisconnect, conn.connectionId);
            else if(!serverDesconectando)
            {
                serverDesconectando = true;
                SupportSingleton.Instance.InvokeInRealTime(() =>
                {
                    serverDesconectando = false;
                }, 2);
            }

            base.OnServerDisconnect(conn);

            
        }
    }
}
