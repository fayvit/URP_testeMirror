using FayvitUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class A_RoomListOption : AnOption
{
    [SerializeField] private Text nameOfPlayer;
    [SerializeField] private InputField inputForName;
    [SerializeField] private Text status;
    [SerializeField] private Text latencia;
    [SerializeField] private Button btnEditar;
    [SerializeField] private Button btnKick;
    [SerializeField] private Button btnPronto;

    public void SetValues(string nomeDoJogador,string status,string latencia,bool isServer, bool isReady,bool owner)
    {
        if (owner && !isReady)
        {
            nameOfPlayer.gameObject.SetActive(false);
            inputForName.gameObject.SetActive(true);
        }

        if (owner)
            inputForName.text = nomeDoJogador;
        else
            inputForName.gameObject.SetActive(false);

        nameOfPlayer.text = nomeDoJogador;
        this.status.text = status;
        this.latencia.text = latencia;

        
        btnKick.gameObject.SetActive(isServer&&!owner);
        btnPronto.gameObject.SetActive(owner);
        btnEditar.gameObject.SetActive(isReady);

        btnKick.onClick.RemoveAllListeners();
        btnKick.onClick.AddListener(Kick);

        btnPronto.onClick.RemoveAllListeners();
        btnPronto.onClick.AddListener(Pronto);

        btnEditar.onClick.RemoveAllListeners();
        btnEditar.onClick.AddListener(Editar);
    }

    void Pronto()
    {
        btnPronto.gameObject.SetActive(false);
        btnEditar.gameObject.SetActive(true);
    }

    void Editar()
    {
        btnPronto.gameObject.SetActive(true);
        btnEditar.gameObject.SetActive(false);
    }

    void Kick()
    { 
    
    }
}
