using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FayvitUI;
using UnityEngine.UI;
using FayvitSupportSingleton;
using MyTestMirror;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class PanelPlayerList : InteractiveUiBase
{
    [SerializeField] private GameObject infoWindow;
    [SerializeField] private Text statusText;
    private System.Action<int> acao;
    private bool estadoDeAcao;

    private Dictionary<string, ServerStatus> listActive;

    public Dictionary<string, ServerStatus> ListActive { get => listActive; private set => listActive = value; }

    #region Editor
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PanelPlayerList), true)]
    public class PanelPlayerListEditor : InteractiveUiBaseEditor
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return EditorGUI.GetPropertyHeight(property);
            if (property.isExpanded)
            {
                SerializedProperty colorModify = property.FindPropertyRelative("colorModify");
                SerializedProperty spriteModify = property.FindPropertyRelative("spriteModify");

                if (colorModify.boolValue && spriteModify.boolValue)
                    return 310;
                else
                    return 245;
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
                SerializedProperty iW = prop.FindPropertyRelative("infoWindow");
                SerializedProperty sText = prop.FindPropertyRelative("statusText");

                
                EditorGUI.PropertyField(new Rect(pos.position + (-base.GetPropertyHeight(prop, label)) * Vector2.down, new Vector2(pos.width, 18)), iW);
                EditorGUI.PropertyField(new Rect(pos.position + (-20 - base.GetPropertyHeight(prop, label)) * Vector2.down, new Vector2(pos.width, 18)), sText);
                
                EditorGUI.EndProperty();
            }
        }
    }

#endif
    #endregion

    public void ReceiveServerTick(Dictionary<string, ServerStatus> list,System.Action<int> acao)
    {
        listActive = list;
        FinishHud();
        StartHud(list.Values.ToList().Count,acao);

        infoWindow.SetActive(list.Count <= 0);
    }

    public void SetStatusText(string s)
    {
        statusText.text = s;
    }

    public void StartHud(int quantidade,System.Action<int> acao)
    {
        
        this.acao += (int x) =>
        {
            if (!estadoDeAcao)
            {
                estadoDeAcao = true;
                ChangeSelectionTo(x);

                SupportSingleton.Instance.InvokeInRealTime(() =>
                {
                    Debug.Log("Função chamada com delay para destaque do botão");
                    acao(x);
                    estadoDeAcao = false;
                }, .05f);
            }
        };

        base.StartHud(quantidade);
    }

    public override void SetContainerItem(GameObject G, int indice)
    {
        A_ListServerOption also = G.GetComponent<A_ListServerOption>();
        ServerStatus ss = listActive.Values.ToList()[indice];
        also.SetThisAction(acao);
        also.ServerName.text = ss.title;
        also.Jogadores.text = ss.players + " / " + ss.capacity;
        also.Latencia.text = ss.ping.time.ToString();
        also.IpAddress.text = ss.ip;
    }

    protected override void AfterFinisher()
    {
        acao = null;
    }

    
}
