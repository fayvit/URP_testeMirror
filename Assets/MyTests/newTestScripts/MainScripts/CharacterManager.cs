using FayvitCam;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FayvitEventAgregator;
using System;

namespace MyTestMirror
{
    public enum EstadoDoPersonagem
    { 
        aPasseio,
        emDano
    }

    public class CharacterManager : NetworkBehaviour
    {
        [SerializeField] private GameObject markPoint;
        [SerializeField] private ControlledMoveForCharacter thisControl;
        [SerializeField] private DamageManager thisDamage;
        [SerializeField] private float distanciaChecaMovimento = 1.2f;

        private NetworkIdentity nId;
        private EstadoDoPersonagem estado = EstadoDoPersonagem.aPasseio;

        // Start is called before the first frame update
        void Start()
        {
            thisControl.StartFields(transform);
            nId = GetComponent<NetworkIdentity>();

            if (nId.isLocalPlayer)
            {
                CameraAplicator.cam.NewFocusForBasicCam(transform, 20, 20);
                EventAgregator.Publish(new GameEvent(EventKey.desligarHudPhoton));
                NetworkClient.RegisterHandler<KnockbackMessage>(OnReceiveKnockback);
            }
        }

        private void OnDestroy()
        {
            NetworkServer.UnregisterHandler<KnockbackMessage>();
        }

        private void OnReceiveKnockback(NetworkConnection arg1, KnockbackMessage arg2)
        {
            SerializableVector3 projPos = (SerializableVector3)arg2.MySendObjects[0];
            uint idDono = (uint)arg2.MySendObjects[1];

            thisDamage.StartDamage(projPos.GetV3, thisControl.Mov);
            estado = EstadoDoPersonagem.emDano;

            //Debug.Log((int)arg2.MySendObjects[0] + " : " + (string)arg2.MySendObjects[1] + " : " + (bool)arg2.MySendObjects[2]);
        }

        void EstadoA_Passeio()
        {
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

            if (thisControl.UpdatePosition(distanciaChecaMovimento))
            {
                markPoint.SetActive(false);
            }
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
                }
            }
        }

        [Command]
        void CmdShoot(Vector3 V)
        {
            Vector3 pos = transform.position + 2*transform.forward;
            Quaternion rot = Quaternion.LookRotation(V);
            BulletBehaviour G2 = Instantiate(NetworkManager.singleton.spawnPrefabs[1].GetComponent<BulletBehaviour>(),pos , rot);
            G2.Dono = gameObject;
            NetworkServer.Spawn(G2.gameObject);
        }
    } 
}
