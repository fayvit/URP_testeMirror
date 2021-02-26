using System.Collections.Generic;
using UnityEngine;
using Mirror;
using FayvitEventAgregator;
using MyTestMirror;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System;

public class Command_RPC_Manager : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        EventAgregator.AddListener(EventKey.bulletDamage, OnPlayerReceiveBulletDamage);
    }


    private void OnDestroy()
    {
        EventAgregator.RemoveListener(EventKey.bulletDamage, OnPlayerReceiveBulletDamage);
    }

    private void OnPlayerReceiveBulletDamage(IGameEvent obj)
    {
        Vector3 pos = (Vector3)obj.MySendObjects[0];
        GameObject danado = (GameObject)obj.MySendObjects[1];
        if (danado.GetComponent<CharacterManager>())
        {
            Vector3 forw = (Vector3)obj.MySendObjects[3];
            NetworkIdentity nId = danado.GetComponent<NetworkIdentity>();
            NetworkIdentity idDono = ((GameObject)obj.MySendObjects[2]).GetComponent<NetworkIdentity>();
            NetworkServer.SendToClientOfPlayer(nId, new StandardDamageMessage() { 
                MySendObjects = { 
                    new SerializableVector3(forw), 
                    idDono.netId
                } });
        }

        RpcBulletView(pos);
    }

    

    [ClientRpc]
    void RpcBulletView(Vector3 pos)
    {
        Destroy(
            Instantiate(
                Resources.Load<GameObject>("particulaDoDano"),
                pos,
                Quaternion.identity
                ),3
            );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class ChangePlayerNameMessage : MyGameMessage
{
    public ChangePlayerNameMessage() : base() { }

}

public class StandardDamageMessage : MyGameMessage
{
    public StandardDamageMessage() : base() { }
    
}

public abstract class MyGameMessage : IMessageBase
{

    public List<object> MySendObjects=new List<object>();

    public MyGameMessage()
    {
        
    }

    public void SetElements(params object[] o)
    {
        MySendObjects.AddRange(o);
    }

    public void Deserialize(NetworkReader reader)
    {
        
        ArraySegment<byte> b = reader.ReadBytesAndSizeSegment();
        ObjectWithBytes(b.ToArray());
    }

    public void Serialize(NetworkWriter writer)
    {

        byte[] bytes = ObjectForBytes();
        
        ArraySegment<byte> b = new ArraySegment<byte>(bytes);
        writer.WriteBytesAndSizeSegment(b);

    }

    public byte[] ObjectForBytes()
    {
        MemoryStream ms = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(ms, MySendObjects);

        return ms.ToArray();
    }

    public void ObjectWithBytes(byte[] b)
    {
        MemoryStream ms = new MemoryStream(b);
        BinaryFormatter bf = new BinaryFormatter();
        MySendObjects = (List<object>)bf.Deserialize(ms);
    }
}
