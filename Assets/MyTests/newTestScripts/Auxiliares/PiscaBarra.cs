using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PiscarBarra
{

    private Image faltouEstamina;
    private bool piscar = false;
    private bool opacidadeCrescente = true;
    private float opacidadeAtual = 0;
    private float tempoPiscando = 1;
    private float tempoDecorrido = 0;
    private float maxOpacity = .75f;
    private float minOpacity = 0;

    private float velocidadeDePisca = 10;

    public PiscarBarra(Image raw, float velocidade = 10)
    {
        faltouEstamina = raw;
        velocidadeDePisca = velocidade;
    }

    public void AcionarPiscaEstamina()
    {
        piscar = true;
        tempoDecorrido = 0;
    }

    // Update is called once per frame
    public void PiscarComTempo()
    {
        Color C = faltouEstamina.color;
        if (piscar)
        {
            tempoDecorrido += Time.deltaTime;


            if (tempoDecorrido < tempoPiscando)
            {

                Piscador();

            }
            else
            {
                opacidadeAtual = 0;
                piscar = false;
            }


        }
        faltouEstamina.color = new Color(C.r, C.g, C.b, opacidadeAtual);
    }

    public void PiscarSemTempo()
    {
        Color C = faltouEstamina.color;
        Piscador();
        faltouEstamina.color = new Color(C.r, C.g, C.b, opacidadeAtual);
    }

    public void SetMinOpacity()
    {
        Color C = faltouEstamina.color;
        faltouEstamina.color = new Color(C.r, C.g, C.b, minOpacity);
    }

    public void SetOpacityZero()
    {
        Color C = faltouEstamina.color;
        faltouEstamina.color = new Color(C.r, C.g, C.b, 0);
    }

    public void Piscador()
    {
        Debug.Log("piscador");
        if (opacidadeCrescente)
        {
            
            opacidadeAtual = Mathf.Lerp(opacidadeAtual, maxOpacity, velocidadeDePisca * Time.deltaTime);
            if (opacidadeAtual > 0.99f*maxOpacity)
            {
                opacidadeAtual = maxOpacity;
                opacidadeCrescente = false;
            }

            Debug.Log("crescente: "+opacidadeAtual);

        }
        else
        {
            opacidadeAtual = Mathf.Lerp(opacidadeAtual, minOpacity, velocidadeDePisca * Time.deltaTime);
            if (opacidadeAtual < 0.01f+minOpacity)
            {
                opacidadeAtual = minOpacity;
                opacidadeCrescente = true;
            }
            Debug.Log("decrescente: " + opacidadeAtual);

        }
    }
}
