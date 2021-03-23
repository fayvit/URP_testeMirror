using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FayvitEventAgregator;
using Mirror;
using FayvitSupportSingleton;
using System;

public class ViewDatesHud : MonoBehaviour
{
    [SerializeField] private Image hpImage;
    [SerializeField] private Image stImage;
    [SerializeField] private Text playerName;
    [SerializeField] private Image doPiscador;

    [SerializeField] private PiscarBarra pisca;

    private bool piscando;

    // Use this for initialization
    void Start()
    {
        pisca = new PiscarBarra(doPiscador);
        EventAgregator.AddListener(EventKey.changeLifePoints, OnChangeLifePoints);
        EventAgregator.AddListener(EventKey.changeStaminaPoint, OnChangeStaminaPoints);
        EventAgregator.AddListener(EventKey.changePlayerName, OnChangePlayerName);
        EventAgregator.AddListener(EventKey.zeroedStamina, OnZeroedStamina);
        EventAgregator.AddListener(EventKey.recoveryZeroedStamina, OnRecoveryZeroedStamina);

        //NetworkIdentity nid = transform.parent.GetComponent<NetworkIdentity>();

        SupportSingleton.Instance.InvokeOnCountFrame(() =>
        {
            EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.requestChangeDates);
        },5);
    }


    private void OnDestroy()
    {
        EventAgregator.RemoveListener(EventKey.changeLifePoints, OnChangeLifePoints);
        EventAgregator.RemoveListener(EventKey.changeStaminaPoint, OnChangeStaminaPoints);
        EventAgregator.RemoveListener(EventKey.changePlayerName, OnChangePlayerName);
        EventAgregator.RemoveListener(EventKey.zeroedStamina, OnZeroedStamina);
        EventAgregator.RemoveListener(EventKey.recoveryZeroedStamina, OnRecoveryZeroedStamina);
    }

    private void OnRecoveryZeroedStamina(IGameEvent obj)
    {
        Transform T = NetworkIdentity.spawned[(uint)obj.MySendObjects[0]].transform;
        if ( T== transform.parent)
        {
            piscando = false;
            pisca.SetOpacityZero();
        }
    }

    private void OnZeroedStamina(IGameEvent obj)
    {
        Transform T = NetworkIdentity.spawned[(uint)obj.MySendObjects[0]].transform;
        if (T == transform.parent)
        {
            piscando = true;   
        }
    }

    private void OnChangeStaminaPoints(IGameEvent obj)
    {
        NetworkIdentity nId = NetworkIdentity.spawned[(uint)obj.MySendObjects[0]];

        //Transform T = NetworkManager.singleton.sp
        if (nId.transform == transform.parent)
        {
            int lp = (int)obj.MySendObjects[1];
            int mlp = (int)obj.MySendObjects[2];
            stImage.fillAmount = (float)lp / mlp;
        }
    }

    private void OnChangeLifePoints(IGameEvent obj)
    {
        NetworkIdentity nId = NetworkIdentity.spawned[(uint)obj.MySendObjects[0]];
        //ClientScene.spawnableObjects[(ulong)obj.MySendObjects[0]];

        //Transform T = NetworkManager.singleton.sp
        if (nId.transform == transform.parent)
        {
            int lp = (int)obj.MySendObjects[1];
            int mlp = (int)obj.MySendObjects[2];
            hpImage.fillAmount = (float)lp / mlp;
        }
    }

    private void OnChangePlayerName(IGameEvent obj)
    {
        if (obj.MySendObjects[1] as Transform == transform.parent)
        {
            playerName.text = (string)obj.MySendObjects[0];
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (piscando)
            pisca.PiscarSemTempo();
    }
}
