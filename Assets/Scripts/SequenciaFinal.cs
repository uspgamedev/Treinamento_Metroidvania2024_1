using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SequenciaFinal : MonoBehaviour
{
    [SerializeField] private bool isFinalSequence = false;
    [SerializeField] private GameObject[] VagalumesFinais;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFinalSequence)
        {

        }
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        isFinalSequence = true;
        foreach (GameObject objeto in VagalumesFinais)
        {
            objeto.SetActive(true);
        }
    }
}
