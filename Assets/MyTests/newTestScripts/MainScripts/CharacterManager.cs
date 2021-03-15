using FayvitCam;
using Mirror;
using UnityEngine;
using FayvitEventAgregator;
using FayvitMove;
using System;

namespace MyTestMirror
{
    public enum EstadoDoPersonagem
    { 
        aPasseio,
        emDano,
        emAtk,
        emMgAtk
    }

    public class CharacterManager : NetworkBehaviour
    {
        [SerializeField] private GameObject markPoint;
        [SerializeField] private ControlledMoveForCharacter thisControl;
        [SerializeField] private SimpleKnowbackManager thisDamage;
        [SerializeField] private DadosDoPersonagem dados;
        [SerializeField] private AttackManager atkManager;
        [SerializeField] private MagicAttackManager mgAttack;
        [SerializeField] private float distanciaChecaMovimento = 1.2f;
        [SerializeField, SyncVar] private string nomeJogador = "Jogador n";
        [SerializeField, SyncVar] private int connectionID = -1;
        [SerializeField] private bool timedDamage = false;
        [SerializeField] private float intervalTimedDamage = .25f;

        private NetworkIdentity nId;
        private EstadoDoPersonagem estado = EstadoDoPersonagem.aPasseio;
        private float contadorDoTempo = 0;

        
        public int ConnectionID { get => connectionID; set => connectionID = value; }

        // Start is called before the first frame update
        void Start()
        {
            thisControl.StartFields(transform);
            dados.StManager.RestartStaminaTimeCount();

            nId = GetComponent<NetworkIdentity>();

            if (nId.isLocalPlayer)
            {
                
                CameraAplicator.cam.NewFocusForBasicCam(transform, 25, 10);
                EventAgregator.Publish(new GameEvent(EventKey.starterCharacterManager));
                EventAgregator.Publish(new GameEvent(EventKey.requestServerEvent,
                    EventKey.starterInServerCharacterManager,connectionID));

                EventAgregator.AddListener(EventKey.sendChangePlayerName, OnChangePlayerName);
                EventAgregator.AddListener(EventKey.enterInTimedDamage, OnEnterInTimedDamage);
                EventAgregator.AddListener(EventKey.exitInTimedDamage, OnExitInTimedDamage);
                EventAgregator.AddListener(EventKey.requestChangeDates, OnRequestChangeDates);

                NetworkClient.RegisterHandler<StandardDamageMessage>(OnReceiveStandardDamage);

                dados.StManager.OnChangeStaminaPoints += () => {
                    EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent,EventKey.changeStaminaPoint,
                        nId.netId, dados.StManager.StaminaPoints, dados.StManager.MaxStaminaPoints);
                };
            }

            
        }

        private void OnDestroy()
        {
            if (nId.isLocalPlayer)
            {
                EventAgregator.RemoveListener(EventKey.sendChangePlayerName, OnChangePlayerName);
                EventAgregator.RemoveListener(EventKey.enterInTimedDamage, OnEnterInTimedDamage);
                EventAgregator.RemoveListener(EventKey.exitInTimedDamage, OnExitInTimedDamage);
                EventAgregator.RemoveListener(EventKey.requestChangeDates, OnRequestChangeDates);
                NetworkClient.UnregisterHandler<StandardDamageMessage>();                
            }
        }

        private void OnRequestChangeDates(IGameEvent obj)
        {
            CmdName(nomeJogador);
            EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.changeLifePoints,
                nId.netId, dados.LifePoints, dados.MaxLifePoints);
            EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.changeStaminaPoint,
                nId.netId, dados.StManager.StaminaPoints, dados.StManager.MaxStaminaPoints);
        }

        private void OnExitInTimedDamage(IGameEvent e)
        {
            timedDamage = false;
            contadorDoTempo = 0;
        }

        private void OnEnterInTimedDamage(IGameEvent e)
        {
            timedDamage = true;
        }

        #region Suprimido
        //private void OnAddNewPlayer(IGameEvent obj)
        //{
        //    NetworkConnection nCon = (NetworkConnection)obj.MySendObjects[0];
        //    if (nCon.identity == nId)
        //        CmdName(nomeJogador);
        //}
        #endregion

        [Command]
        void CmdName(string nome)
        {
            Debug.Log("Chegou aqui o nome: " + nome);
            if (string.IsNullOrEmpty(nome))
                nome = "Player: " + NetworkManager.singleton.numPlayers;

            RpcT(nome);
        }

        [ClientRpc]
        void RpcT(string nome)
        {
            Debug.Log("Chegou aqui RPC: " + nome);
            nomeJogador = nome;
            EventAgregator.Publish(new GameEvent(EventKey.changePlayerName, nomeJogador, transform));
        }

        void EstadoA_Passeio()
        {
            atkManager.UpdateAttack();
            mgAttack.UpdateAttack();
            bool run = Input.GetKey(KeyCode.E) && dados.StManager.VerifyStaminaAction();

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Input");
                Vector3 V;
                if (GetRaycastPoint.GetPoint(out V))
                {
                    Debug.Log("GetRaycastPoint");
                    thisControl.ModificarOndeChegar(V);
                    markPoint.transform.parent = null;
                    markPoint.SetActive(true);
                    markPoint.transform.position = V;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Q) && mgAttack.IniciarAtaqueSePodeAtacar())
            {
                Vector3 V;
                if (!GetRaycastPoint.GetPoint(out V))
                {
                    V = transform.forward;
                }
                else
                {
                    V = Vector3.ProjectOnPlane(V - transform.position, Vector3.up).normalized;
                }

                CmdShoot(V);
                mgAttack.DisparaAtaque();
                estado = EstadoDoPersonagem.emMgAtk;
                thisControl.Mov.UseSlowSpeed = true;
            }
            else if (Input.GetKeyDown(KeyCode.W)
                && atkManager.IniciarAtaqueSePodeAtacar()
                && mgAttack.IniciarAtaqueSePodeAtacar())
            {

                //thisControl.ModificarOndeChegar(transform.position);
                CmdIniciarAtk();
                thisControl.Mov.UseSlowSpeed = true;

            }

            if (thisControl.UpdatePosition(distanciaChecaMovimento, run))
            {
                markPoint.SetActive(false);
                dados.StManager.StaminaRegen(false);
            }
            else
            {
                dados.StManager.StaminaRegen(run);

            }



        }

        internal void SetName(string playerName)
        {
            CmdName(playerName);
        }

        private void OnChangePlayerName(IGameEvent e)
        {
            CmdName((string)e.MySendObjects[0]);
        }

        private void OnReceiveStandardDamage(NetworkConnection arg1, StandardDamageMessage arg2)
        {
            SerializableVector3 projPos = (SerializableVector3)arg2.MySendObjects[0];
            uint idDono = (uint)arg2.MySendObjects[1];

            thisDamage.StartDamage(projPos.GetV3, thisControl.Mov);
            estado = EstadoDoPersonagem.emDano;

            dados.ApplyDamage(10);
            //CmdUpdateLifePoints(dados.LifePoints, dados.MaxLifePoints);
            EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.changeLifePoints, nId.netId, 
                dados.LifePoints, dados.MaxLifePoints
                );
        }

        #region commandSuprimidos
        //[Command]
        //void CmdUpdateLifePoints(int lp, int mlp)
        //{
        //    RpcUpdateLifePoints(lp, mlp);
        //}

        //[ClientRpc]
        //void RpcUpdateLifePoints(int lp, int mlp)
        //{
        //    EventAgregator.Publish(new GameEvent(EventKey.changeLifePoints, transform, lp, mlp));
        //}

        //[Command]
        //void CmdChangeStaminaPoints(int st,int mst)
        //{
        //    RpcChangeStaminaPoints(st, mst);
        //}

        //[ClientRpc]
        //void RpcChangeStaminaPoints(int st, int mst)
        //{
        //    EventAgregator.Publish(new GameEvent(EventKey.changeStaminaPoint, transform, st, mst));
        //}
        #endregion

        [Command]
        void CmdIniciarAtk()
        {
            RpcIniciarAtk();
        }

        [ClientRpc]
        void RpcIniciarAtk()
        {
            atkManager.DisparaAtaqueComum();
            estado = EstadoDoPersonagem.emAtk;
        }

        [Command]
        void CmdShoot(Vector3 V)
        {
            Vector3 pos = transform.position + V+1.75f*Vector3.up;
            Quaternion rot = Quaternion.LookRotation(V);
            BulletBehaviour G2 = Instantiate(NetworkManager.singleton.spawnPrefabs[3].GetComponent<BulletBehaviour>(), pos, rot);
            G2.Dono = gameObject;
            NetworkServer.Spawn(G2.gameObject);
        }

        [Command]
        void CmdResetAtkManager()
        {
            RpcResetAtkManager();
        }

        [ClientRpc]
        void RpcResetAtkManager()
        {
            atkManager.ResetaAttackManager();
            estado = EstadoDoPersonagem.aPasseio;
        }

        // Update is called once per frame
        void Update()
        {
            if (nId.isLocalPlayer)
            {
                switch (estado)
                {
                    case EstadoDoPersonagem.aPasseio:
                        EstadoA_Passeio();
                    break;
                    case EstadoDoPersonagem.emDano:
                        if (thisDamage.UpdateDamage())
                        {
                            estado = EstadoDoPersonagem.aPasseio;
                        }
                    break;
                    case EstadoDoPersonagem.emAtk:
                        thisControl.UpdatePosition(distanciaChecaMovimento);
                        if (atkManager.UpdateAttack())
                        {
                            CmdResetAtkManager();
                            thisControl.Mov.UseSlowSpeed = false;
                        }
                    break;
                    case EstadoDoPersonagem.emMgAtk:
                        thisControl.UpdatePosition(distanciaChecaMovimento);
                        if (mgAttack.UpdateAttack())
                        {
                            mgAttack.ResetaAttackManager();
                            estado = EstadoDoPersonagem.aPasseio;
                            thisControl.Mov.UseSlowSpeed = false;
                        }
                    break;
                }

                VerifyApplyTimedDamage();
            }


        }

        void VerifyApplyTimedDamage()
        {
            if (timedDamage)
            {
                contadorDoTempo -= Time.deltaTime;
                if (contadorDoTempo < 0)
                {
                    contadorDoTempo = intervalTimedDamage;
                    dados.ApplyDamage(4);
                    //CmdUpdateLifePoints(dados.LifePoints, dados.MaxLifePoints);
                    EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.changeLifePoints, nId.netId,
                            dados.LifePoints, dados.MaxLifePoints
                        );
                    CmdView(transform.position);
                }
            }
        }

        [Command]
        void CmdView(Vector3 pos)
        {
            EventAgregator.PublishGameEvent(EventKey.requestViewFiredamage, pos);
        }
    
    } 
}

