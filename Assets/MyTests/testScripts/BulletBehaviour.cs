using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using FayvitEventAgregator;

public class BulletBehaviour : NetworkBehaviour
{
    [SerializeField] private float vel = 10;
    [SerializeField] private float tempoDeVida = 10;

    private float contadorDeTempo = 0;
    public GameObject Dono { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * vel * Time.deltaTime;
        
        contadorDeTempo += Time.deltaTime;

        if(contadorDeTempo>tempoDeVida)
            NetworkServer.Destroy(gameObject);

    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != Dono)
        {
            
            EventAgregator.Publish(new GameEvent(EventKey.bulletDamage,transform.position,other.gameObject,Dono,transform.forward));

            NetworkServer.Destroy(gameObject);
        }
    }

    
}
