using FayvitMove;
using FayvitEventAgregator;
using UnityEngine;

[System.Serializable]
public class SimpleKnowbackManager
{
    [SerializeField] private float tempoNoDano = .25f;
    [SerializeField] private float repulsaoNoDano = 1;
    [SerializeField] private float levitar = 1;

    private float tempoDecorrido = 0;
    private Vector3 dirDeRepulsao = Vector3.zero;
    private BasicMove mov;

    public void StartDamage(Vector3 dirDano, BasicMove mov)
    {
        this.mov = mov;

        tempoDecorrido = 0;
        dirDeRepulsao = dirDano.normalized;

        EventAgregator.Publish(new GameEvent(EventKey.requestShakeCam, ShakeAxis.xy, 5, .25f));

    }

    public bool UpdateDamage()
    {
        bool retorno = false;
        tempoDecorrido += Time.deltaTime;

        if (tempoDecorrido < tempoNoDano)
        {

            mov.Controller.Move(dirDeRepulsao * repulsaoNoDano + Vector3.up * levitar);

        }
        else
        {
            retorno = true;
        }
        return retorno;
    }
}