using FayvitUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class A_PanelLisrPlayerOption : AnOption
{
    [SerializeField] private Text nomeDoJogador;
    [SerializeField] private Text status;
    [SerializeField] private Text latencia;
    [SerializeField] private Button btnKick;
    [SerializeField] private Button btnPronto;
    [SerializeField] private Button btnReEdit;

    private System.Action<int> kickAction;

    public void SetarOpcao(
        System.Action<int> readyAction,
        System.Action<int> kickAction,
        string nomeJ,
        string status,
        string latencia,
        bool kickActive,
        bool prontoActive,
        bool reEditActive
        )
    {
        ThisAction += readyAction;
        this.kickAction += kickAction;

        nomeDoJogador.text = nomeJ;
        this.status.text = status;
        this.latencia.text = latencia;

        btnKick.gameObject.SetActive(kickActive);
        btnPronto.gameObject.SetActive(prontoActive);
        btnReEdit.gameObject.SetActive(reEditActive);

    }

    public void BtnKick()
    {
        kickAction?.Invoke(transform.GetSiblingIndex() - 1);
    }
}
