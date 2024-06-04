using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private Image title;
    private TextMeshProUGUI text;
    private Image blackFade;

    private bool canStart = false;

    void Start()
    {
        title = transform.Find("Title").GetComponent<Image>();
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        blackFade = transform.Find("BlackFade").GetComponent<Image>();
        StartCoroutine(TitleSequence());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canStart) {
            StartCoroutine(Iniciar());
        }
    }

    private IEnumerator TitleSequence() {
        yield return new WaitForSeconds(2.5f);

        title.DOFade(1f, 2.5f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(4f);

        text.DOFade(1f, 2.5f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(2f);

        canStart = true;
    }

    private IEnumerator Iniciar() {

        text.DOFade(0f, 1f);

        yield return new WaitForSeconds(2f);

        title.DOFade(0f, 2.5f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(3.5f);

        SceneManager.LoadScene("MainMenu");
    }
}
