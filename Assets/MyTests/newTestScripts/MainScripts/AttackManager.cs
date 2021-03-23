using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackManager
{
#pragma warning disable 0649
    [SerializeField] private GameObject colisorDoAtaqueComum;
    [SerializeField] private AudioClip[] swordSound;
    private float tempoDecorrido = 0;
    private bool estouAtacando = false;
    private AttacksTipes tipo = AttacksTipes.neutro;

    private const float tempoDoAtaqueComum = 0.25f;
    private const float INTERVALO_DE_ATAQUE = .4f;
#pragma warning restore 0649

    public uint CostStaminaPoints { get; set; } = 45;

    private enum AttacksTipes
    {
        neutro = -1,
        comum,
    }

    public void AttackIntervalUpdate()
    {
        if (!estouAtacando)
            tempoDecorrido += Time.deltaTime;
    }

    public bool IniciarAtaqueSePodeAtacar()
    {
        if (tempoDecorrido > INTERVALO_DE_ATAQUE)
        {
            InserirSomdaLamina();
            estouAtacando = true;
            tempoDecorrido = 0;
            return true;
        }

        return false;
    }

    public void DisparaAtaqueComum()
    {
        //  if (IniciarAtaqueSePodeAtacar())
        {
            tipo = AttacksTipes.comum;
            colisorDoAtaqueComum.SetActive(true);
        }

    }




    public void InserirSomdaLamina()
    {
        int qual = Random.Range(0, swordSound.Length);

        //EventAgregator.Publish(new StandardSendGameEvent(EventKey.disparaSom, swordSound[qual]));
    }

    public bool UpdateAttack()
    {
        tempoDecorrido += Time.deltaTime;
        switch (tipo)
        {
            case AttacksTipes.comum:
                if (tempoDecorrido > tempoDoAtaqueComum)
                {
                    ResetaAttackManager();
                    return true;
                }
            break;
        }
        return false;
    }

    public void ResetaAttackManager()
    {
        tempoDecorrido = 0;
        estouAtacando = false;
        colisorDoAtaqueComum.SetActive(false);
        tipo = AttacksTipes.neutro;
    }
}