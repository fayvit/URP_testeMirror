using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirecaoNoPlano
{
    public static Vector3 NoUp(Vector3 origem, Vector3 extremidade)
    {
        return Vector3.ProjectOnPlane(extremidade - origem, Vector3.up);
    }

    public static Vector3 NoUpNormalizado(Vector3 origem, Vector3 extremidade)
    {
        return NoUp(origem, extremidade).normalized;
    }

    public static Vector3 RandomDir
    {
        get { return NoUpNormalizado(Vector3.zero, Random.insideUnitSphere); }
    }
}