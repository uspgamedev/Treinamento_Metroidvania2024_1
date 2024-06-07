using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;
using DG.Tweening;

public class Ending : MonoBehaviour
{
    [SerializeField] private Light2D entranceLight;
    [SerializeField] private Canvas endingCanvas;

    [SerializeField] private Image whitePanel;

    [SerializeField] private TextMeshProUGUI phrase1;

    [SerializeField] private TextMeshProUGUI phrase2;

    [SerializeField] private TextMeshProUGUI phraseThanks;

    Image[] hpCanvasImages;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            hpCanvasImages = GameObject.Find("CanvasHP").GetComponentsInChildren<Image>();
            foreach (Image hp in hpCanvasImages) {
                hp.DOFade(0f, 2f).SetEase(Ease.InOutSine);
            }
            GameObject timerCanvas = GameObject.Find("Canvas Timer 1");
            if (timerCanvas != null)
                timerCanvas.SetActive(false);
            StartCoroutine(Sequence());
        }
    }

    private IEnumerator Sequence() {
        DOTween.To(() => entranceLight.intensity, (x) => entranceLight.intensity = x, 15f, 7f).SetEase(Ease.InOutSine);
        DOTween.To(() => entranceLight.pointLightOuterRadius, (x) => entranceLight.pointLightOuterRadius = x, 35f, 7f).SetEase(Ease.InOutSine);
        DOTween.To(() => entranceLight.pointLightInnerRadius, (x) => entranceLight.pointLightInnerRadius = x, 18f, 10f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(5f);

        whitePanel.DOFade(1f, 5f);

        yield return new WaitForSeconds(9f);

        phrase1.DOFade(1f, 2.5f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(6f);

        phrase2.DOFade(1f, 2.5f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(8f);

        phrase1.DOFade(0f, 2.5f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(6f);

        phrase2.DOFade(0f, 2.5f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(6f);

        phraseThanks.DOFade(1f, 2.5f);
    }
}
