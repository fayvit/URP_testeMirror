using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FayvitUI;

public class A_ListServerOption : AnOption
{
    [SerializeField] private Text serverName;
    [SerializeField] private Text jogadores;
    [SerializeField] private Text latencia;
    [SerializeField] private Text ipAddress;

    public Text ServerName { get => serverName; set => serverName = value; }
    public Text Jogadores { get => jogadores; set => jogadores = value; }
    public Text Latencia { get => latencia; set => latencia = value; }
    public Text IpAddress { get => ipAddress; set => ipAddress = value; }

    public void SetThisAction(System.Action<int> acao) { ThisAction += acao; }
}
