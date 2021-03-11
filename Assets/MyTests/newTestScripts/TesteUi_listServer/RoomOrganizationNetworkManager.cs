using FayvitEventAgregator;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTestMirror
{
    public class RoomOrganizationNetworkManager : NetworkManager
    {
        public override void OnClientConnect(NetworkConnection conn)
        {
            ClientScene.AddPlayer(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            GameObject player = Instantiate(spawnPrefabs[0]);
            Debug.Log(conn.connectionId);
            player.GetComponent<PlayerSoulFromNetwork>().ConnectionID = conn.connectionId;
            NetworkServer.AddPlayerForConnection(conn, player);

            GameObject commSender = Instantiate(spawnPrefabs[1]);
            NetworkServer.Spawn(commSender, conn);
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

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            Debug.Log("disconnect?");
            EventAgregator.PublishGameEvent(EventKey.requestServerEvent, EventKey.playerDisconnect, conn.connectionId);
            base.OnServerDisconnect(conn);

            
        }
    }
}
