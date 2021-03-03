using FayvitEventAgregator;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationListener : MonoBehaviour
{
    private Animator meuAnimator;
    private NetworkIdentity nid;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        nid = GetComponent<NetworkIdentity>();
        
        if (nid.isLocalPlayer)
        {
            meuAnimator = GetComponent<Animator>();
            EventAgregator.AddListener(EventKey.changeMoveSpeed, OnChangeMoveSpeed);
        }
        else
            enabled = false;
    }

    private void OnChangeMoveSpeed(IGameEvent obj)
    {
        if (((GameObject)obj.MySendObjects[0]).gameObject == gameObject)
        {
            float f = ((Vector3)obj.MySendObjects[1]).magnitude;
            meuAnimator.SetFloat("Vel", f);
        }
    }

    private void OnDestroy()
    {
        EventAgregator.RemoveListener(EventKey.changeMoveSpeed, OnChangeMoveSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
