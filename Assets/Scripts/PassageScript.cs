using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassageScript : MonoBehaviour
{
    [SerializeField] private string cenaAlvo;
    [SerializeField] private float duracaoFade = 1f;

    private bool encostou = false;
    private bool trocarCena = false;
    private SpriteRenderer fadeRenderer;
    private Color corInicial;

    private int transFrame;

    private void Start()
    {
        GameObject fadeObjeto = GameObject.FindGameObjectWithTag("BlackFade"); 
        transFrame = 0;
        if (fadeObjeto != null)
        {
            fadeRenderer = fadeObjeto.GetComponent<SpriteRenderer>();
            corInicial = fadeRenderer.color;
            fadeRenderer.color = new Color(corInicial.r, corInicial.g, corInicial.b, 0); //Existe alguma forma de mudar só o alpha?????
        }
    }

    private void FixedUpdate()
    {
        if (encostou && !trocarCena)
        {
            float alpha = Mathf.Lerp(0, 255, transFrame / 60);
            fadeRenderer.color = new Color(corInicial.r, corInicial.g, corInicial.b, alpha);

            if (alpha >= 250)
            {
                trocarCena = true;
                SceneManager.LoadScene(cenaAlvo);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            encostou = true;
        }
    }
}
