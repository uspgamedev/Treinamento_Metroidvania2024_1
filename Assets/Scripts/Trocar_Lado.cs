using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trocar_Lado : MonoBehaviour
{
    private enum LadoInicial {
        A,
        B
    }

    private GameObject[] listaA;
    private GameObject[] listaB;
    private bool canChangeSides;
    private Collider2D coll;

    [SerializeField] LadoInicial ladoInicial;
    [SerializeField] float transTime;
    private Image whiteFade;
    private float alpha;
    private bool toFade = false;

    void Start()
    {
        listaA = GameObject.FindGameObjectsWithTag("LadoA");
        listaB = GameObject.FindGameObjectsWithTag("LadoB");

        whiteFade = GameObject.Find("WhiteFade").GetComponent<Image>();
        whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, 0f);

        if (ladoInicial == LadoInicial.A) {
            foreach (GameObject objeto in listaB)
            {
                objeto.SetActive(false);
            }
        }
        if (ladoInicial == LadoInicial.B) {
            foreach (GameObject objeto in listaA)
            {
                objeto.SetActive(false);
            }
        }

        canChangeSides = false;
    }

    void Update()
    {
        //muda de lado se o player esta dentro do local de mudar de lado e se o player pressionou a tecla E
        if(canChangeSides)
        {
            if(coll.gameObject.tag == "Player") 
            {
                if (Input.GetKeyDown(KeyCode.E)) 
                {
                    StartCoroutine(ChangeSides());
                }
            }
        }

        if (toFade) {
            coll.GetComponent<Health>().Fade(transTime, whiteFade, 1f, alpha);
        }
        else {
            if (whiteFade.color.a > 0f) {
                whiteFade.color = new Color (0f, 0f, 0f, 0f);
            }
        }
    }

    private IEnumerator ChangeSides() //habilita e desabilita objetos de acordo com o lado para o qual deve ser mudado
    {
        toFade = true;
        coll.GetComponent<Player_Movement>().canMove2 = false;

        yield return new WaitForSeconds(transTime);

        coll.GetComponent<Player_Movement>().canMove2 = true;

        foreach (GameObject objeto in listaA)
        {
            objeto.SetActive(!objeto.activeInHierarchy);
        }
        foreach (GameObject objeto in listaB)
        {
            objeto.SetActive(!objeto.activeInHierarchy);
        }

        yield return new WaitForSeconds(transTime);

        toFade = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canChangeSides = true;
        coll = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canChangeSides = false;
    }
}
