using FayvitUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class RoomListPanel : InteractiveUiBase
{
    [SerializeField] private Text serverName;
    [SerializeField] private GameObject painelAguardandoConexao;
    [SerializeField] private GameObject painelTodosProntos;

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
                    return 330;
                else
                    return 265;
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


                EditorGUI.PropertyField(new Rect(pos.position + (-base.GetPropertyHeight(prop, label)) * Vector2.down, new Vector2(pos.width, 18)), iW);
                EditorGUI.PropertyField(new Rect(pos.position + (-20 - base.GetPropertyHeight(prop, label)) * Vector2.down, new Vector2(pos.width, 18)), pAgua);
                EditorGUI.PropertyField(new Rect(pos.position + (-40-base.GetPropertyHeight(prop, label)) * Vector2.down, new Vector2(pos.width, 18)), pProntos);

                EditorGUI.EndProperty();
            }
        }
    }
#endif
    #endregion 

    public void StartHud()
    {
        base.StartHud(0);
    }

    public override void SetContainerItem(GameObject G, int indice)
    {
        A_RoomListOption aRoom = G.GetComponent<A_RoomListOption>();
        TesteCreationUIForListServer.PlayerDates L = l[indice];
        bool owner = NetworkIdentity.spawned[(uint)L.netId].isLocalPlayer;
        //Debug.Log(L.connectionID + " ; " + NetworkClient.connection.connectionId + " : " + owner+" : "+ NetworkConnection.LocalConnectionId);
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

    
}
