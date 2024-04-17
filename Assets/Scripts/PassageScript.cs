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

    private int transFrame = 0;

    private void FixedUpdate()
    {
        if (encostou && !trocarCena)
        {
            transFrame++;
            float alpha = Mathf.Lerp(255, 0, transFrame / (60 * duracaoFade));
            

            if (alpha >= 250)
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
