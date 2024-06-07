using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SequenciaFinal : MonoBehaviour
{
    [SerializeField] private bool isFinalSequence = false;
    [SerializeField] private GameObject vagalumesFinais;
    [SerializeField] private GameObject objetoGameOver;
    [SerializeField] private float timerMax = 120f;
    [SerializeField] private float timer = 60f;
    [SerializeField] private GameObject textCanvas;
    [SerializeField] private GameObject firstTextBox;
    [SerializeField] private GameObject panelFirstText;

    [SerializeField] private GameObject textBox;
    private TextMeshProUGUI text;
    [SerializeField] private GameObject panel1;
    private SupportScript support;

    [SerializeField] private GameObject endingTrigger;

    // Start is called before the first frame update
    void Start()
    {
        text = textBox.GetComponent<TextMeshProUGUI>();
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
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
        }
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        isFinalSequence = true;
        vagalumesFinais.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = false;
        textCanvas.SetActive(true);
        endingTrigger.SetActive(true);
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        panel1.SetActive(false);
        textBox.SetActive(false);
        yield return new WaitForSeconds(8);
        panelFirstText.SetActive(false);
        firstTextBox.SetActive(false);
        panel1.SetActive(true);
        textBox.SetActive(true);
        timer = timerMax;
    }
}
