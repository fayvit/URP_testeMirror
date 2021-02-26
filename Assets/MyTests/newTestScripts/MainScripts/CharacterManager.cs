using FayvitCam;
using Mirror;
using UnityEngine;
using FayvitEventAgregator;
using System;

namespace MyTestMirror
{
    public enum EstadoDoPersonagem
    { 
        aPasseio,
        emDano,
        emAtk
    }

    public class CharacterManager : NetworkBehaviour
    {
        [SerializeField] private GameObject markPoint;
        [SerializeField] private ControlledMoveForCharacter thisControl;
        [SerializeField] private SimpleKnowbackManager thisDamage;
        [SerializeField] private DadosDoPersonagem dados;
        [SerializeField] private AttackManager atkManager;
        [SerializeField] private float distanciaChecaMovimento = 1.2f;
        [SerializeField, SyncVar] private string nomeJogador = "Jogador n";

        private NetworkIdentity nId;
        private EstadoDoPersonagem estado = EstadoDoPersonagem.aPasseio;

        // Start is called before the first frame update
        void Start()
        {
            thisControl.StartFields(transform);
            dados.StManager.RestartStaminaTimeCount();

            nId = GetComponent<NetworkIdentity>();

            if (nId.isLocalPlayer)
            {

                CameraAplicator.cam.NewFocusForBasicCam(transform, 30, 10);
                EventAgregator.Publish(new GameEvent(EventKey.desligarHudMirror));
                NetworkClient.RegisterHandler<StandardDamageMessage>(OnReceiveStandardDamage);
                EventAgregator.AddListener(EventKey.sendChangePlayerName, OnChangePlayerName);
                NetworkClient.RegisterHandler<ChangePlayerNameMessage>(OnRequestChangeName);

                dados.StManager.OnChangeStaminaPoints += () => {
                    Debug.Log("Chamando onChange");
                    CmdChangeStaminaPoints(dados.StManager.StaminaPoints, dados.StManager.MaxStaminaPoints);
                };

            }

            //EventAgregator.AddListener(EventKey.addNewPlayer, OnAddNewPlayer);


        }

        private void OnDestroy()
        {
            if (nId.isLocalPlayer)
            {
                NetworkClient.UnregisterHandler<StandardDamageMessage>();
                EventAgregator.RemoveListener(EventKey.sendChangePlayerName, OnChangePlayerName);
                NetworkClient.UnregisterHandler<ChangePlayerNameMessage>();

            }

            //EventAgregator.RemoveListener(EventKey.addNewPlayer, OnAddNewPlayer);


        }

        private void OnRequestChangeName(NetworkConnection arg1, ChangePlayerNameMessage arg2)
        {
            if ((int)arg2.MySendObjects[0] != nId.connectionToClient.connectionId)
            {
                CmdName(nomeJogador);
            }
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
            if (string.IsNullOrEmpty(nome))
                nome = "Player: " + NetworkManager.singleton.numPlayers;

            RpcT(nome);
        }

        [ClientRpc]
        void RpcT(string nome)
        {

            nomeJogador = nome;
            EventAgregator.Publish(new GameEvent(EventKey.changePlayerName, nomeJogador, transform));
        }

        private void OnChangePlayerName(IGameEvent e)
        {
            CmdName((string)e.MySendObjects[1]);
        }

        private void OnReceiveStandardDamage(NetworkConnection arg1, StandardDamageMessage arg2)
        {
            SerializableVector3 projPos = (SerializableVector3)arg2.MySendObjects[0];
            uint idDono = (uint)arg2.MySendObjects[1];

            thisDamage.StartDamage(projPos.GetV3, thisControl.Mov);
            estado = EstadoDoPersonagem.emDano;

            dados.ApplyDamage(10);
            CmdUpdateLifePoints(dados.LifePoints, dados.MaxLifePoints);
        }

        [Command]
        void CmdUpdateLifePoints(int lp, int mlp)
        {
            RpcUpdateLifePoints(lp, mlp);
        }

        [ClientRpc]
        void RpcUpdateLifePoints(int lp, int mlp)
        {
            EventAgregator.Publish(new GameEvent(EventKey.changeLifePoints, transform, lp, mlp));
        }

        void EstadoA_Passeio()
        {
            atkManager.UpdateAttack();
            bool run = Input.GetKey(KeyCode.E) && dados.StManager.VerifyStaminaAction();

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 V;
                if (GetRaycastPoint.GetPoint(out V))
                {
                    thisControl.ModificarOndeChegar(V);
                    markPoint.transform.parent = null;
                    markPoint.SetActive(true);
                    markPoint.transform.position = V;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Q))
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
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (atkManager.IniciarAtaqueSePodeAtacar())
                {
                    thisControl.ModificarOndeChegar(transform.position);
                    CmdIniciarAtk();
                }
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
        [Command]
        void CmdChangeStaminaPoints(int st,int mst)
        {
            RpcChangeStaminaPoints(st, mst);
        }

        [ClientRpc]
        void RpcChangeStaminaPoints(int st, int mst)
        {
            EventAgregator.Publish(new GameEvent(EventKey.changeStaminaPoint, transform, st, mst));
        }

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
            Vector3 pos = transform.position + transform.forward;
            Quaternion rot = Quaternion.LookRotation(V);
            BulletBehaviour G2 = Instantiate(NetworkManager.singleton.spawnPrefabs[1].GetComponent<BulletBehaviour>(), pos, rot);
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
                        if (atkManager.UpdateAttack())
                        {
                            CmdResetAtkManager();
                        }
                    break;
                }
            }
        }
    } 
}
