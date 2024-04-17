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

    private int transFrame = 0;

    private void FixedUpdate()
    {
        if (encostou && !trocarCena)
        {
            transFrame++;
            float alpha = Mathf.Lerp(255, 0, transFrame / (60*duracaoFade));
            SetAlphaForAllRenderers(alpha, true);

            if (alpha >= 250)
            {
                trocarCena = true;
                SceneManager.LoadScene(cenaAlvo);
            }
        }
    }

    private void SetAlphaForAllRenderers(float alpha, bool decrease)
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if ((renderer.material.color.a > alpha && decrease) || (renderer.material.color.a < alpha && !decrease))
            {
                Color cor = renderer.material.color;
                renderer.material.color = new Color(cor.r, cor.g, cor.b, alpha);
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
