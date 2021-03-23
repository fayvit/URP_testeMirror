using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using FayvitEventAgregator;

public class DamageTrigger : NetworkBehaviour
{

    public GameObject Dono { get; set; }

    private void Start()
    {
        Dono = transform.parent.gameObject;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != Dono)
        {
            EventAgregator.Publish(
                new GameEvent(
                    EventKey.bulletDamage, 
                    transform.position, 
                    other.gameObject, 
                    Dono, 
                    transform.forward
                    ));
        }
    }
}
