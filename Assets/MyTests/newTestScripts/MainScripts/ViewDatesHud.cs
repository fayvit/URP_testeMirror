using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FayvitEventAgregator;
using System;
using Mirror;
using FayvitSupportSingleton;
using System.Data.SqlTypes;

public class ViewDatesHud : MonoBehaviour
{
    [SerializeField] private Image hpImage;
    [SerializeField] private Image stImage;
    [SerializeField] private Text playerName;

    // Use this for initialization
    void Start()
    {
        EventAgregator.AddListener(EventKey.changeLifePoints, OnChangeLifePoints);
        EventAgregator.AddListener(EventKey.changeStaminaPoint, OnChangeStaminaPoints);
        EventAgregator.AddListener(EventKey.changePlayerName, OnChangePlayerName);

        //NetworkIdentity nid = transform.parent.GetComponent<NetworkIdentity>();

        SupportSingleton.Instance.InvokeOnEndFrame(() =>
        {
            EventAgregator.PublishGameEvent(EventKey.networkSendEvent, EventKey.requestChangeDates);
        });
    }


    private void OnDestroy()
    {
        EventAgregator.RemoveListener(EventKey.changeLifePoints, OnChangeLifePoints);
        EventAgregator.RemoveListener(EventKey.changeStaminaPoint, OnChangeStaminaPoints);
        EventAgregator.RemoveListener(EventKey.changePlayerName, OnChangePlayerName);
    }

    private void OnChangeStaminaPoints(IGameEvent obj)
    {
        NetworkIdentity nId = NetworkIdentity.spawned[(uint)obj.MySendObjects[0]];

        Debug.Log(nId);

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

        Debug.Log(nId);

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

    }
}
