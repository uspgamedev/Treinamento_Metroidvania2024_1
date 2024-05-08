using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trocar_Lado : MonoBehaviour
{
    public GameObject[] listaA;
    public GameObject[] listaB;
    private bool canChangeSides;
    private Collider2D coll;
    // Start is called before the first frame update
    void Start()
    {
        canChangeSides = false;
        foreach (GameObject objeto in listaB) //disabilita todos os objetos do lado B no começo do jogo
        {
            objeto.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //muda de lado se o player esta dentro do local de mudar de lado e se o player pressionou a tecla E
        if(canChangeSides)
        {
            if(coll.gameObject.tag == "Player") 
            {
                if (Input.GetKeyDown(KeyCode.E)) 
                {
                    ChangeSides();
                }
            }
        }
    }

    private void ChangeSides() //habilita e desabilita objetos de acordo com o lado para o qual deve ser mudado
    {
        foreach (GameObject objeto in listaA)
        {
            objeto.SetActive(!objeto.activeInHierarchy);
        }
        foreach (GameObject objeto in listaB)
        {
            objeto.SetActive(!objeto.activeInHierarchy);
        }
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
