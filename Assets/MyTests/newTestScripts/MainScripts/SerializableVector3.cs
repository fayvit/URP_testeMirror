using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct SerializableVector3
{
    private float x;
    private float y;
    private float z;

    public SerializableVector3(Vector3 V)
    {
        x = V.x;
        y = V.y;
        z = V.z;
    }

    public Vector3 GetV3 { get => new Vector3(x, y, z); }
}