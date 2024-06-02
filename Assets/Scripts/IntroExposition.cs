using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class IntroExposition : MonoBehaviour
{
    [SerializeField] private int boxNumber;

    private GameObject textCanvas;
    private GameObject textBox;
    private TextMeshProUGUI text;
    private GameObject panel1;
    private GameObject panel2;
    private GameObject panel3;

    private int panelNum = 1;
    private float textWait = 3.5f;

    private SupportScript support;

    void Start()
    {   
        textCanvas = GameObject.Find("CanvasIntro");
        panel1 = textCanvas.transform.Find("Panel 1").gameObject;
        panel2 = textCanvas.transform.Find("Panel 2").gameObject;
        panel3 = textCanvas.transform.Find("Panel 3").gameObject;
        panel1.GetComponent<Image>().enabled = true;
        panel2.GetComponent<Image>().enabled = true;
        panel3.GetComponent<Image>().enabled = true;
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(false);

        textBox = textCanvas.transform.Find("Intro Text").gameObject;
        text = textBox.GetComponent<TextMeshProUGUI>();

        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") {
            if (support.textCoroutine != null) {
                StopCoroutine(support.textCoroutine);
                textBox.SetActive(false);
                panel1.SetActive(false);
                panel2.SetActive(false);
                panel3.SetActive(false);
                support.textCoroutine = null;
            }
            ChangeText();
            support.textCoroutine = StartCoroutine(TextActivate(panelNum));
        }
    }

    private void ChangeText() {
        switch (boxNumber) {
            case 1:
                text.text = "The cave was dark, but with just enough lighting for the miner to enter it.";
                break;
            case 2:
                text.text = "No darkness, of course, would be enough to stop him.";
                textWait = 3.5f;
                break;
            case 3:
                text.text = "He was looking, after all, for gold!";
                panelNum = 3;
                break;
            case 4:
                text.text = "There was undoubtedly something eerie about the cave, though.";
                textWait = 4f;
                panelNum = 1;
                break;
            case 5:
                text.text = "Upon entering, the guy wondered why no one had ever spoken to him about it. It was almost as if no one's ever even seen it.";
                textWait = 8f;
                panelNum = 2;
                break;
            case 6:
                text.text = "Heck, not even the miner himself remembered since when or why it was there. It just was.";
                textWait = 8f;
                panelNum = 2;
                break;
            case 7:
                text.text = "Even though he would eventually find them here, however, he wasn't looking for answers.";
                textWait = 8f;
                panelNum = 2;
                break;
            case 8:
                text.text = "He was looking for gold, of course!";
                textWait = 8f;
                panelNum = 3;
                break;
        }
    }

    private IEnumerator TextActivate(int panel) {
        switch(panel) {
            case 1:
                panel1.SetActive(true);
                break;
            case 2:
                panel2.SetActive(true);
                break;
            case 3:
                panel3.SetActive(true);
                break;
        }

        textBox.SetActive(true);

        yield return new WaitForSeconds(textWait);

        textBox.SetActive(false);
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(false);
        gameObject.SetActive(false);

        support.textCoroutine = null;
    }
}
