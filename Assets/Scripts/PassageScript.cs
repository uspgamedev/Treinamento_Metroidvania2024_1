using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Adicionando a diretiva using para o namespace UnityEngine.UI

public class PassageScript : MonoBehaviour
{
    [SerializeField] private string cenaAlvo;
    [SerializeField] private float duracaoFade = 1f;
    private bool encostou = false;
    private bool trocarCena = false;
    private int transFrame = 0;
    private GameObject blackFade;

    private bool defaded = false;

    private void Start()
    {
        blackFade = GameObject.FindGameObjectWithTag("BlackFade");
        defaded = true;
        transFrame = 0;
    }

    private void FixedUpdate()
    {
        if (defaded)
        {
            transFrame++;
            float alpha = Mathf.Lerp(1f, 0f, (float)transFrame / (60 * duracaoFade));

            Image image = blackFade.GetComponent<Image>();

            if (image != null)
            {
                image.color = new Color(0f, 0f, 0f, alpha);
            }

            if (alpha <= 0)
            {
                transFrame = 0;
                defaded = false;
            }
        }

        if (encostou && !trocarCena)
        {
            transFrame++;
            float alpha = Mathf.Lerp(0f, 1f, (float) transFrame / (60 * duracaoFade));

            Image image = blackFade.GetComponent<Image>();

            if (image != null)
            {
                image.color = new Color(0f, 0f, 0f, alpha);
            }

            if (alpha >= 1f)
            {
                trocarCena = true;
                SceneManager.LoadScene(cenaAlvo, LoadSceneMode.Single);
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
