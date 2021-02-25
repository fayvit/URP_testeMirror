using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BasicMove_b : NetworkBehaviour
{
    [SerializeField] private GameObject G = default;
    NetworkIdentity nId;
    // Start is called before the first frame update
    void Start()
    {
        
        nId = GetComponent<NetworkIdentity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (nId.isLocalPlayer)
        {
            transform.position += new Vector3(
                Input.GetAxis("Horizontal"),
                0,
               Input.GetAxis("Vertical")
                ) * Time.deltaTime * 10;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CmdShoot();
            }

            
        }
    }

    [Command]
    void CmdShoot()
    {
        GameObject G2 = Instantiate(G, transform.position+transform.forward, Quaternion.identity);
        NetworkServer.Spawn(G2);
    }
}
