using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using FayvitEventAgregator;

public class TriggerForTimedDamage : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkIdentity nID = other.GetComponent<NetworkIdentity>();

            if (nID.isLocalPlayer)
                EventAgregator.PublishGameEvent(EventKey.enterInTimedDamage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkIdentity nID = other.GetComponent<NetworkIdentity>();

            if (nID.isLocalPlayer)
                EventAgregator.PublishGameEvent(EventKey.exitInTimedDamage);
        }
    }
}
