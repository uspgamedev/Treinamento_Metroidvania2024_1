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
    [SerializeField] private GameObject objetoGameOver;
    [SerializeField] private float timerMax = 120f;
    [SerializeField] private float timer = 60f;
    [SerializeField] private GameObject textCanvas;
    [SerializeField] private GameObject textBox;
    private TextMeshProUGUI text;
    private GameObject panel1;
    private SupportScript support;

    // Start is called before the first frame update
    void Start()
    {
        textCanvas = GameObject.Find("CanvasTimer");
        textBox = textCanvas.transform.Find("Text (TMP)").gameObject;
        text = textBox.GetComponent<TextMeshProUGUI>();
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        textCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFinalSequence)
        {
            timer -= Time.deltaTime;
            text.text = timer.ToString("0.#");
            if (objetoGameOver.GetComponent<GameOverMenu>().isGameOver == true)
            {
                timer = timerMax;
            }

            if (timer <= 0f) 
            {
                GameObject.Find("Player").GetComponent<Health>().currentHealth = 0;
            }
            //If timer to 0, set player life to zero
        }
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        isFinalSequence = true;
        foreach (GameObject objeto in VagalumesFinais)
        {
            objeto.SetActive(true);
        }
        GetComponent<BoxCollider2D>().enabled = false;
        textCanvas.SetActive(true);
        timer = timerMax;
    }
}
