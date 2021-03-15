using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimacaoDeSpriteParaUI : MonoBehaviour
{
    public SpriteRenderer meuSprite;
    public Image minhaImagem;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        minhaImagem.sprite = meuSprite.sprite;
    }
}
