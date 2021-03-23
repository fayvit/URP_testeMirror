using FayvitUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using FayvitSupportSingleton;
using FayvitEventAgregator;
using MyTestMirror;
using System.Net.Sockets;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class RoomListPanel : InteractiveUiBase
{
    [SerializeField] private Text serverName;
    [SerializeField] private GameObject painelAguardandoConexao;
    [SerializeField] private GameObject painelTodosProntos;
    [SerializeField] private GameObject BotaoIniciar;
    [SerializeField] private Text infoText;

    private List<TesteCreationUIForListServer.PlayerDates> l;

    #region Editor
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(RoomListPanel), true)]
    public class RoomListPanelEditor : InteractiveUiBaseEditor
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return EditorGUI.GetPropertyHeight(property);
            if (property.isExpanded)
            {
                SerializedProperty colorModify = property.FindPropertyRelative("colorModify");
                SerializedProperty spriteModify = property.FindPropertyRelative("spriteModify");

                if (colorModify.boolValue && spriteModify.boolValue)
                    return 370;
                else
                    return 305;
            }
            else
                return 20;
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {

            base.OnGUI(pos, prop, label);

            if (prop.isExpanded)
            {
                EditorGUI.BeginProperty(pos, label, prop);
                SerializedProperty iW = prop.FindPropertyRelative("serverName");
                SerializedProperty pAgua = prop.FindPropertyRelative("painelAguardandoConexao");
                SerializedProperty pProntos = prop.FindPropertyRelative("painelTodosProntos"); 
                    SerializedProperty bIniciar = prop.FindPropertyRelative("BotaoIniciar"); 
                    SerializedProperty bInfoText = prop.FindPropertyRelative("infoText"); 


                EditorGUI.PropertyField(new Rect(pos.position + (-base.GetPropertyHeight(prop, label)) * Vector2.down, new Vector2(pos.width, 18)), iW);
                EditorGUI.PropertyField(new Rect(pos.position + (-20 - base.GetPropertyHeight(prop, label)) * Vector2.down, new Vector2(pos.width, 18)), pAgua);
                EditorGUI.PropertyField(new Rect(pos.position + (-40-base.GetPropertyHeight(prop, label)) * Vector2.down, new Vector2(pos.width, 18)), pProntos);
                EditorGUI.PropertyField(new Rect(pos.position + (-60 - base.GetPropertyHeight(prop, label)) * Vector2.down, new Vector2(pos.width, 18)), bIniciar);
                EditorGUI.PropertyField(new Rect(pos.position + (-80 - base.GetPropertyHeight(prop, label)) * Vector2.down, new Vector2(pos.width, 18)), bInfoText);

                EditorGUI.EndProperty();
            }
        }
    }
#endif
    #endregion 

    public void StartHud()
    {
        
        BotaoIniciar.SetActive(NetworkServer.active);

        string s = "<color=cyan>Insira seu nome para o jogo e clique em pronto para aguardar o inicio da partida </color>";
        EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.changeRoomInfoText, s);


        base.StartHud(0);
        l = new List<TesteCreationUIForListServer.PlayerDates>();
    }

    public override void SetContainerItem(GameObject G, int indice)
    {
        A_RoomListOption aRoom = G.GetComponent<A_RoomListOption>();
        TesteCreationUIForListServer.PlayerDates L = l[indice];
        bool owner = NetworkIdentity.spawned[L.netId].isLocalPlayer;
        aRoom.SetValues(L.playerName,L.pronto?"Pronto":"Na sala",L.latencia,NetworkServer.active,L.pronto,owner);

    }

    

    internal void RestartHud(List<TesteCreationUIForListServer.PlayerDates> l)
    {
        this.l = l;
        FinishHud();
        base.StartHud(l.Count);

        
        painelAguardandoConexao.SetActive(l.Count<=0);
    }

    protected override void AfterFinisher()
    {
        
    }

    internal void IniciarEstadoDeInicioDeJogo()
    {
        int cont = 10;

        RecursiveInvoke(cont);
    }

    public void ChangeInfoText(string s)
    {
        infoText.text = s;
    }

    void RecursiveInvoke(int cont)
    {
        bool foi = true;
        for (int i = 0; i < l.Count; i++)
            foi &= l[i].pronto;

        SupportSingleton.Instance.InvokeInRealTime(() =>
        {
            string s = "<color=red>O jogo iniciará em: " + cont+"</color>";
            EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.changeRoomInfoText, s);
            cont--;

            if (cont < 0 || foi)
            {

                NetPlaySceneLoader.IniciarCarregamento("ForLoadGameScene", "MyListServerScene",l);
                SingletonServerTick.Instance.FinishServerTick();
                
            }
            else if (l[0].pronto)
            {
                RecursiveInvoke(cont);
            }
            else if (!l[0].pronto)
            {
                s = "<color=cyan>Insira seu nome para o jogo e clique em pronto para aguardar o inicio da partida </color>";
                EventAgregator.PublishGameEvent(EventKey.networkSendRpcEvent, EventKey.changeRoomInfoText, s);
            }

        }, 1);
    }
}
