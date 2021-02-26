using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FayvitEventAgregator;
using System;

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
    }

    private void OnChangeStaminaPoints(IGameEvent obj)
    {
        if (obj.MySendObjects[0] as Transform == transform.parent)
        {
            int lp = (int)obj.MySendObjects[1];
            int mlp = (int)obj.MySendObjects[2];
            stImage.fillAmount = (float)lp / mlp;
        }
    }

    private void OnChangeLifePoints(IGameEvent obj)
    {
        if (obj.MySendObjects[0] as Transform == transform.parent)
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

    private void OnDestroy()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
