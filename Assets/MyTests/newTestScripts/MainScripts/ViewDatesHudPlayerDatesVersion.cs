using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FayvitEventAgregator;
using Mirror;

public class ViewDatesHudPlayerDatesVersion : MonoBehaviour
{
    [SerializeField] private Image hpImage;
    [SerializeField] private Image stImage;
    [SerializeField] private Text playerName;


    // Start is called before the first frame update
    void Start()
    {
        EventAgregator.AddListener(EventKey.changeLifePoints, OnChangeLifePoints);
        EventAgregator.AddListener(EventKey.changeStaminaPoint, OnChangeStaminaPoints);
        EventAgregator.AddListener(EventKey.changePlayerName, OnChangePlayerName);
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
}
