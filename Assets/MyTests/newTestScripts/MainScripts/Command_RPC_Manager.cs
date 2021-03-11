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
        EventAgregator.AddListener(EventKey.requestViewFiredamage, OnPlayerReceiveFireDamage);
    }


    private void OnDestroy()
    {
        EventAgregator.RemoveListener(EventKey.bulletDamage, OnPlayerReceiveBulletDamage);
        EventAgregator.RemoveListener(EventKey.requestViewFiredamage, OnPlayerReceiveFireDamage);
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

        RpcBulletView(pos, "particulaDoDano");
    }

    private void OnPlayerReceiveFireDamage(IGameEvent e)
    {
        Vector3 pos = (Vector3)e.MySendObjects[0];
        RpcBulletView(pos, "particulaDoDano_fogo");
    }

    [ClientRpc]
    void RpcBulletView(Vector3 pos,string particula)
    {
        Destroy(
            Instantiate(
                Resources.Load<GameObject>(particula),
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
        MySendObjects = BytesToObject.ObjectWithBytes(b.ToArray());
    }

    public void Serialize(NetworkWriter writer)
    {

        byte[] bytes = BytesToObject.ObjectForBytes(MySendObjects);
        
        ArraySegment<byte> b = new ArraySegment<byte>(bytes);
        writer.WriteBytesAndSizeSegment(b);

    }

    
}

public static class BytesToObject
{
    public static byte[] OneObjectForBytes(object MySendObjects)
    {
        return ObjectForBytes(new List<object>() { MySendObjects });
    }

    public static byte[] ObjectForBytes(List<object> MySendObjects)
    {
        MemoryStream ms = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(ms, MySendObjects);

        return ms.ToArray();
    }

    public static T OneObjectWithBytes<T>(byte[] b)
    {
        return (T)ObjectWithBytes(b)[0];
    }

    public static List<object> ObjectWithBytes(byte[] b)
    {
        MemoryStream ms = new MemoryStream(b);
        BinaryFormatter bf = new BinaryFormatter();
        return (List<object>)bf.Deserialize(ms);
    }
}
