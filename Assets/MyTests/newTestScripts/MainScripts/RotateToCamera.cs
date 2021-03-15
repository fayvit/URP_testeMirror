using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    private Transform daCamera;
    // Start is called before the first frame update
    void Start()
    {
        daCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (daCamera == null)
            Start();

        transform.rotation = Quaternion.LookRotation(daCamera.forward);
    }
}
