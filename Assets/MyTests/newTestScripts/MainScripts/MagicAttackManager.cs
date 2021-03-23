using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MagicAttackManager
{
#pragma warning disable 0649
    [SerializeField] private AudioClip[] triggerSound;
    private float tempoDecorrido = 0;
    private bool estouAtacando = false;
    private AttacksTipes tipo = AttacksTipes.neutro;

    private const float TEMPO_DO_ATAQUE_COMUM = 0.05f;
    private const float INTERVALO_DE_ATAQUE = .3f;
#pragma warning restore 0649

    public uint CostStaminaPoints { get; set; } = 25;

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
            InserirSom();
            estouAtacando = true;
            tempoDecorrido = 0;
            return true;
        }

        return false;
    }

    public void DisparaAtaque()
    {
        //  if (IniciarAtaqueSePodeAtacar())
        {
            tipo = AttacksTipes.comum;
            
        }

    }

    public void InserirSom()
    {
        int qual = Random.Range(0, triggerSound.Length);

        //EventAgregator.Publish(new StandardSendGameEvent(EventKey.disparaSom, swordSound[qual]));
    }

    public bool UpdateAttack()
    {
        tempoDecorrido += Time.deltaTime;
        switch (tipo)
        {
            case AttacksTipes.comum:
                if (tempoDecorrido > TEMPO_DO_ATAQUE_COMUM)
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
        tipo = AttacksTipes.neutro;
    }
}