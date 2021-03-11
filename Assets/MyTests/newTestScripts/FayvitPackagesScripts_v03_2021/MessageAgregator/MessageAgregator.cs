using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FayvitMessageAgregator
{
    public static class MessageAgregator<T>  where T:IMessageBase,new()
    {
        private static Dictionary<Type, List<Action<T>>> _eventDictionary
            = new Dictionary<Type, List<Action<T>>>();

        public static void AddListener(Action<T> callback)
        {
            List<Action<T>> callbackList;
            if (!_eventDictionary.TryGetValue(typeof(T), out callbackList))
            {
                callbackList = new List<Action<T>>();
                _eventDictionary.Add(typeof(T), callbackList);
            }

            callbackList.Add(callback);
        }

        public static void RemoveListener(Action<T> acao)
        {
            List<Action<T>> callbackList;
            if (_eventDictionary.TryGetValue(typeof(T), out callbackList))
            {
                callbackList.Remove(acao);
            }
        }

        public static void Publish()
        {
            Publish(new T());
        }

        public static void Publish(T umEvento)
        {
            List<Action<T>> callbackList;
            if (_eventDictionary.TryGetValue(typeof(T), out callbackList))
            {
                //Debug.Log(callbackList.Count+" : "+umEvento.Sender+" : "+key);

                foreach (var e in callbackList)
                {
                    if (e != null)
                        e(umEvento);
                    else
                        Debug.LogWarning("Message agregator chamou uma função nula na key: " + typeof(T) +
                            "\r\n Geralmente ocorre quando o objeto do evento foi destruido sem se retirar do listener");
                }
            }
        }

        public static void ClearListeners()
        {
            _eventDictionary = new Dictionary<Type, List<Action<T>>>();
        }
    }

    public interface IMessageBase { }
}
