﻿using UnityEngine;
using System.Collections;

namespace FayvitSupportSingleton
{
    public class SupportSingleton : MonoBehaviour
    {
        //private Dictionary<string, System.Action> schelduleActions = new Dictionary<string, System.Action>();
        private static SupportSingleton instance;

        public static SupportSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject G = new GameObject();
                    G.name = "Fayvit_SupportSingleton";
                    DontDestroyOnLoad(G);

                    instance = G.AddComponent<SupportSingleton>();
                }

                return instance;

            }
        }
        // Use this for initialization
        void Start()
        {
            SupportSingleton[] g = FindObjectsOfType<SupportSingleton>();

            if (g.Length > 1)
                Destroy(gameObject);
            else
                instance = this;
        }

        public void InvokeInRealTime(System.Action acao, float time)
        {
            StartCoroutine(RealTimeCall(time, acao));
        }

        public void InvokeInSeconds(GameObject G,System.Action acao, float time)
        {
            StartCoroutine(TimeCallWithGO(G,time, acao));
        }

        public void InvokeInRealTime(GameObject G,System.Action acao, float time)
        {
            StartCoroutine(RealTimeCallWithGO(G,time, acao));
        }

        public void InvokeInSeconds(System.Action acao, float time)
        {
            StartCoroutine(TimeCall(time, acao));
        }

        public void InvokeOnCountFrame(System.Action acao,uint count=1)
        {
            StartCoroutine(CountFrameInvoke(acao,count));
        }

        public void InvokeOnCountFrame(GameObject G,System.Action acao, uint count = 1)
        {
            StartCoroutine(CountFrameInvoke(G,acao, count));
        }

        IEnumerator CountFrameInvoke(GameObject G,System.Action s, uint count)
        {
            for (int i = 0; i < count; i++)
                yield return new WaitForEndOfFrame();

            if (G != null)
                s();
        }

        IEnumerator CountFrameInvoke(System.Action s,uint count)
        {
            for(int i=0; i<count;i++)
                yield return new WaitForEndOfFrame();

            s();
        }

        IEnumerator EndFrameInvokeWithObject(GameObject G, System.Action s)
        {
            yield return new WaitForEndOfFrame();
            if (G != null)
                s();
        }

        IEnumerator EndFrameInvoke(System.Action s)
        {
            yield return new WaitForEndOfFrame();
            s();
        }


        public void InvokeOnEndFrame(GameObject G, System.Action acao)
        {
            StartCoroutine(EndFrameInvokeWithObject(G,acao));
        }

        public void InvokeOnEndFrame(System.Action acao)
        {
            StartCoroutine(EndFrameInvoke(acao));
        }

        IEnumerator RealTimeCall(float time, System.Action s)
        {
            yield return new WaitForSecondsRealtime(time);
            s();
        }

        IEnumerator RealTimeCallWithGO(GameObject G,float time, System.Action s)
        {
            yield return new WaitForSecondsRealtime(time);
            if (G != null)
                s();
        }

        IEnumerator TimeCall(float time, System.Action s)
        {
            yield return new WaitForSeconds(time);
            s();
        }

        IEnumerator TimeCallWithGO(GameObject G,float time, System.Action s)
        {
            yield return new WaitForSeconds(time);

            if(G!=null)
                s();
        }


    }
}