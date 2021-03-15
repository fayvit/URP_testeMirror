using UnityEngine;
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace FayvitEventAgregator
{
    public class EventAgregator
    {
        private static Dictionary<EventKey, List<Action<IGameEvent>>> _eventDictionary
            = new Dictionary<EventKey, List<Action<IGameEvent>>>();

        public static void AddListener(EventKey key, Action<IGameEvent> callback)
        {
            List<Action<IGameEvent>> callbackList;
            if (!_eventDictionary.TryGetValue(key, out callbackList))
            {
                callbackList = new List<Action<IGameEvent>>();
                _eventDictionary.Add(key, callbackList);
            }

            callbackList.Add(callback);

        }

        public static void RemoveListener(EventKey key, Action<IGameEvent> acao)
        {
            List<Action<IGameEvent>> callbackList;
            if (_eventDictionary.TryGetValue(key, out callbackList))
            {
                callbackList.Remove(acao);
            }
        }

        public static void Publish(EventKey key, IGameEvent umEvento = null)
        {
            List<Action<IGameEvent>> callbackList;
            if (_eventDictionary.TryGetValue(key, out callbackList))
            {
                //Debug.Log(callbackList.Count+" : "+umEvento.Sender+" : "+key);

                foreach (var e in callbackList)
                {
                    if (e != null)
                        e(umEvento);
                    else
                        Debug.LogWarning("Event agregator chamou uma função nula na key: " + key +
                            "\r\n Geralmente ocorre quando o objeto do evento foi destruido sem se retirar do listener");
                }
            }
        }

        public static void PublishGameEvent(object[] o, EventKey key)
        {
            Publish(new GameEvent(o, key));
        }

        public static void PublishGameEvent(EventKey key, params object[] o)
        {
            Publish(new GameEvent(o, key));
        }

        public static void Publish(IGameEvent e)
        {
            Publish(e.Key, e);
        }

        public static void ClearListeners()
        {
            _eventDictionary = new Dictionary<EventKey, List<Action<IGameEvent>>>();
        }
    }

    public enum EventKey
    {
        nulo = -1,
        UiDeOpcoesChange,
        confirmationPanelBtnYes,
        confirmationPanelBtnNo,
        mensagemEnchendo,
        mensgemCheia,
        caixaDeTextoIndo,
        caixaDeTextoSaiu,
        closeMessagePanel,
        requestShakeCam,
        controlableReached,
        requestHideControllers,
        requestShowControllers,
        changeHardwareController,
        animateDownJump,
        animateStartJump,
        animateFall,
        changeMoveSpeed,
        starterCharacterManager,
        conectandoParaJoin,
        entrandoNoLobby,
        entrouNoLobby,
        entrandoNaSala,
        stopClient,
        bulletDamage,
        changePlayerName,
        sendChangePlayerName,
        changeLifePoints,
        changeStaminaPoint,
        networkSendRpcEvent,
        iniciandoConexao,
        conexaoRealizada,
        salaCriada,
        addNewPlayer = 32,
        enterInTimedDamage = 33,
        exitInTimedDamage = 34,
        requestViewFiredamage = 35,
        requestChangeDates = 36,
        backToMainMenu = 37,
        receiveServerTick = 38,
        requestServerEvent = 39,
        enterNewSoulPlayer = 40,
        sendPlayersDates = 41,
        playerDisconnect = 42,
        updateRoomListInfos = 43,
        clickInEditPlayer = 44,
        clickInKickPlayer = 45,
        clickPlayerReady = 46,
        requestSendToOne = 47,
        serverRequestDisconnect = 48,
        changeRoomInfoText = 49,
        changeCommandID = 50,
        clientSceneLoadReady = 51,
        requesChangeTimeScale = 52,
        spawRealPlayer = 53,
        requestLogMessage = 54,
        onEditPlayerNameInListRoom = 55,
        starterInServerCharacterManager = 56,
        ViewParticlesDamage = 57
    }
}