using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportScript : MonoBehaviour
{
    private enum LadoInicial {
        A,
        B
    }

    [HideInInspector] public GameObject[] listaA = new GameObject[15];
    [HideInInspector] public GameObject[] listaB = new GameObject[15];

    [SerializeField] LadoInicial ladoInicial;

    void Awake()
    {
        GameObject[] listaTemp = FindObjectsOfType<GameObject>(true);
        int ia = 0;
        int ib = 0;

        foreach (GameObject obj in listaTemp) {
            if (obj.CompareTag("LadoA")) {
                listaA[ia] = obj;
                ia++;
            }
            else if (obj.CompareTag("LadoB")) {
                listaB[ib] = obj;
                ib++;
            }
        }

        if (ladoInicial == LadoInicial.A) {
            foreach (GameObject objeto in listaA)
            {
                if (objeto != null)
                    objeto.SetActive(true);
            }
            foreach (GameObject objeto in listaB)
            {
                if (objeto != null)
                    objeto.SetActive(false);
            }
        }
        
        if (ladoInicial == LadoInicial.B) {
            foreach (GameObject objeto in listaB)
            {
                if (objeto != null)
                    objeto.SetActive(true);
            }
            foreach (GameObject objeto in listaA)
            {
                if (objeto != null)
                    objeto.SetActive(false);
            }
        }
    }


    void Update()
    {
        
    }
}
