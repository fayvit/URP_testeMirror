using FayvitEventAgregator;
using FayvitSupportSingleton;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyTestMirror
{
    public class NetPlaySceneLoader : MonoBehaviour
    {
        private string cenaAlvo;
        private string descarregar;
        private Dictionary<int, bool> verifiqueCenaAlvoPronta;
        private Dictionary<int, bool> verifiqueLoadSceneDescarregada;
        private Dictionary<int, bool> verifiqueCenaDescarregarDescarregada;
        private List<TesteCreationUIForListServer.PlayerDates> l;

        public static void IniciarCarregamento(
            string nomeCena, 
            string descarregar,
            List<TesteCreationUIForListServer.PlayerDates> l,
            System.Action acaoFinalizadora = null)
        {
            
            GameObject G = new GameObject();
            NetPlaySceneLoader loadScene = G.AddComponent<NetPlaySceneLoader>();

            loadScene.CenaDoCarregamento(nomeCena, descarregar,l,acaoFinalizadora);

            SupportSingleton.Instance.InvokeOnEndFrame(() =>
            {
                EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.requesChangeTimeScale, 0f);
            });
        }

        private void CenaDoCarregamento(string nomeCena,string descarregar,
            List<TesteCreationUIForListServer.PlayerDates> l,
            System.Action acaoFinalizadora)
        {
            this.l = l;
            verifiqueCenaAlvoPronta = new Dictionary<int, bool>();
            verifiqueLoadSceneDescarregada = new Dictionary<int, bool>();
            verifiqueCenaDescarregarDescarregada = new Dictionary<int, bool>();            

            foreach (var i in NetworkServer.connections.Keys)
            {
                verifiqueCenaAlvoPronta[i] = false;
                verifiqueLoadSceneDescarregada[i] = false;
                verifiqueCenaDescarregarDescarregada[i] = false;
            }

            cenaAlvo = nomeCena;
            this.descarregar = descarregar;
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene("LoadScene", LoadSceneMode.Additive);
            SceneManager.sceneLoaded += OnSceneLoaded;
            NetworkServer.SendToAll(new SceneMessage()
            {
                sceneName = "LoadScene",
                sceneOperation = SceneOperation.LoadAdditive
            });


        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            
            string nome = arg0.name;

            if (nome == "LoadScene")
            {
                SceneManager.LoadScene(cenaAlvo,LoadSceneMode.Additive);
            }
            else if (nome == cenaAlvo)
            {

                verifiqueCenaAlvoPronta[0] = true;

                VerifiqueTodosProntos();

            }
        }

        private void OnUnloadScene(Scene arg0) { }
        private void OnUnloadScene(string nome)
        {
            if (nome == "LoadScene" )
            {
                verifiqueLoadSceneDescarregada[0] = true;

                VerifiqueProntosParaInicio();
            }
            else if (nome == descarregar)
            {
                SupportSingleton.Instance.InvokeOnEndFrame(() =>
                {
                    EventAgregator.PublishGameEvent(EventKey.requestServerEvent, EventKey.spawRealPlayer, 0/*connID do Server*/);
                });

                verifiqueCenaDescarregarDescarregada[0] = true;
                VerifiqueProntosParaInicio();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            EventAgregator.AddListener(EventKey.clientSceneLoadReady,OnClientSceneLoadReady);
            EventAgregator.AddListener(EventKey.requesChangeTimeScale, OnRequestChangeTimeScale);
            EventAgregator.AddListener(EventKey.spawRealPlayer, OnRequestSpawnRealPlayer);
            EventAgregator.AddListener(EventKey.starterInServerCharacterManager, OnStarterCharacterController);

        }

        private void OnDestroy()
        {
            EventAgregator.RemoveListener(EventKey.clientSceneLoadReady, OnClientSceneLoadReady);
            EventAgregator.RemoveListener(EventKey.requesChangeTimeScale, OnRequestChangeTimeScale);
            EventAgregator.RemoveListener(EventKey.spawRealPlayer, OnRequestSpawnRealPlayer);
            EventAgregator.RemoveListener(EventKey.starterInServerCharacterManager, OnStarterCharacterController);
        }

        private void OnStarterCharacterController(IGameEvent obj)
        {
            int connID = (int)obj.MySendObjects[0];
            int index = GetPlayerDatesID(connID);

            Debug.Log("Player Name: " + l[index].playerName + " : " + NetworkServer.connections[l[index].connectionID].identity.netId);

            EventAgregator.PublishGameEvent(
                EventKey.requestSendToOne,
                NetworkServer.connections[l[index].connectionID].identity.netId,
                EventKey.sendChangePlayerName,
                l[index].playerName);
        }

        private void OnRequestSpawnRealPlayer(IGameEvent obj)
        {   
            int connID = (int)obj.MySendObjects[0];

            Vector3 pos = new Vector3(
                UnityEngine.Random.Range(-9, 9), 1,
                UnityEngine.Random.Range(-9, 9)
                );

            GameObject G = NetworkManager.singleton.spawnPrefabs[2];
            GameObject player = Instantiate(G, pos, Quaternion.identity);
            CharacterManager cm = player.GetComponent<CharacterManager>();
            cm.ConnectionID = connID;
            int index = GetPlayerDatesID(connID);
            cm.SetName(l[index].playerName);

            NetworkServer.ReplacePlayerForConnection(NetworkServer.connections[connID], player);
        }

        private int GetPlayerDatesID(int connID)
        {
            int guarde = -1;
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].connectionID == connID)
                    guarde = i;
            }

            return guarde;
        }

        private void OnRequestChangeTimeScale(IGameEvent obj)
        {

            float f = (float)obj.MySendObjects[0];
            Time.timeScale = f;

        }

        private void OnClientSceneLoadReady(IGameEvent obj)
        {
            string nome = (string)obj.MySendObjects[0];
            uint netID = (uint)obj.MySendObjects[1];
            SceneOperation op = (SceneOperation)obj.MySendObjects[2];
            NetworkIdentity nId = NetworkIdentity.spawned[netID];
            int connID = nId.GetComponent<PlayerSoulFromNetwork>().ConnectionID;

            if (nome == "LoadScene" && op == SceneOperation.LoadAdditive)
            {
                NetworkServer.SendToClientOfPlayer(nId, new SceneMessage()
                {
                    sceneName = cenaAlvo,
                    sceneOperation = SceneOperation.LoadAdditive
                });
            }
            else if (nome == cenaAlvo)
            {
            
                verifiqueCenaAlvoPronta[connID] = true;

                VerifiqueTodosProntos();

            }
            else if (nome == "LoadScene" && op == SceneOperation.UnloadAdditive)
            {
                verifiqueLoadSceneDescarregada[connID] = true;

                VerifiqueProntosParaInicio();
            }
            else if (nome == descarregar)
            {
                EventAgregator.PublishGameEvent(EventKey.requestServerEvent, EventKey.spawRealPlayer, connID);

                verifiqueCenaDescarregarDescarregada[connID] = true;
                VerifiqueProntosParaInicio();
            }
        }

        void VerifiqueProntosParaInicio()
        {
            bool prontos = true;
            foreach (var i in verifiqueCenaDescarregarDescarregada.Keys)
            {

                prontos &= verifiqueCenaDescarregarDescarregada[i] & verifiqueLoadSceneDescarregada[i];
            }



            if (prontos)
                EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.requesChangeTimeScale, 1f);
        }
     
        void VerifiqueTodosProntos()
        {
            bool prontos = true;
            foreach (var i in verifiqueCenaAlvoPronta.Keys)
                prontos &= verifiqueCenaAlvoPronta[i];

            //SceneManager.sceneUnloaded += OnUnloadScene;
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (prontos)
            {
                if (SceneManager.GetSceneByName(descarregar).isLoaded)
                {
                    SceneManager.UnloadSceneAsync(descarregar);
                    OnUnloadScene(descarregar);


                    NetworkServer.SendToAll(new SceneMessage()
                    {
                        sceneName = descarregar,
                        sceneOperation = SceneOperation.UnloadAdditive
                    });
                }

                if (SceneManager.GetSceneByName("LoadScene").isLoaded)
                {
                    SceneManager.UnloadSceneAsync("LoadScene");
                    OnUnloadScene("LoadScene");


                    NetworkServer.SendToAll(new SceneMessage()
                    {
                        sceneName = "LoadScene",
                        sceneOperation = SceneOperation.UnloadAdditive
                    });
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}