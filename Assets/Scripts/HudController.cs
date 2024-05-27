using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class HudController : MonoBehaviour
{
    private enum InOut
    {
        IN,
        OUT
    }

    private const float NEXT_BUTTON_POSITION = 250;
    private const float BUTTONS_END_Y = 175; // Posição final dos botões
    private const float TWEEN_TIME = 0.5f;
    private const float BLACK_FADE_ALPHA_IN = 0.4f;
    private const float BLACK_FADE_ALPHA_OUT = 1f;
    private const float DELAY_BEFORE_RETURN = 0.4f;
    private const float DELAY_BEFORE_INACTIVATION = 0.6f;
    private GameObject[] menuButtons;
    private Image blackFade;
    private GameObject pauseText;
    private Tween currentTween; // Variável para manter uma referência ao tween atual
    [HideInInspector] public bool isOnPauseMenu = false;

    private void setButtonStatus(bool status)
    {
        pauseText.SetActive(status);
        foreach (var button in menuButtons)
        {
            button.SetActive(status);
        }
    }

    void Awake()
    {
        blackFade = GameObject.Find("BlackFade").GetComponent<Image>();
        pauseText = GameObject.Find("Pause (text)");
        menuButtons = GameObject.FindGameObjectsWithTag("MenuButton")
                    .OrderByDescending(button => button.transform.position.y)
                    .ToArray();

        setButtonStatus(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOnPauseMenu)
            {
                ContinueGame();
            }
            else
            {
                callPauseMenu();
            }
        }
    }

    void callPauseMenu()
    {
        isOnPauseMenu = true;
        setButtonStatus(true);
        Time.timeScale = 0;

        doBlackFadeTween(BLACK_FADE_ALPHA_IN, TWEEN_TIME);
        tweenButtons(InOut.IN);
    }

    private void doBlackFadeTween(float nextAlpha, float tweenTime)
    {
        blackFade.DOFade(nextAlpha, tweenTime).SetEase(Ease.InOutSine).SetUpdate(true);
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        doBlackFadeTween(BLACK_FADE_ALPHA_OUT, TWEEN_TIME);
        tweenButtons(InOut.OUT);
        StartCoroutine(resetObjects(DELAY_BEFORE_RETURN));
    }

    private IEnumerator callToReturn(float timeBeforeReturn)
    {
        yield return new WaitForSeconds(timeBeforeReturn);
        setButtonStatus(false);
        ReturnToMenu();
    }

    private void tweenButtons(InOut direction)
    {
        float targetY = direction == InOut.IN ? BUTTONS_END_Y : BUTTONS_END_Y - NEXT_BUTTON_POSITION;

        float targetAlpha = direction == InOut.IN ? 1f : 0f; // Alpha final desejado

        TextMeshProUGUI pauseTextMeshPro = pauseText.GetComponent<TextMeshProUGUI>();
        Color targetColor = pauseTextMeshPro.color;
        targetColor.a = targetAlpha;

        pauseTextMeshPro.DOKill(); // Cancelar o tween anterior, se existir

        pauseTextMeshPro.DOColor(targetColor, TWEEN_TIME).SetUpdate(true);

        int numButtons = menuButtons.Length;
        float startY = targetY + NEXT_BUTTON_POSITION / 3f * (numButtons - 1) / 2f;

        for (int i = 0; i < numButtons; i++)
        {
            float offsetY = NEXT_BUTTON_POSITION / 2f * i;
            var buttonTransform = menuButtons[i].transform;
            buttonTransform.DOKill(); // Cancelar o tween anterior, se existir
            buttonTransform.DOMoveY(startY - offsetY, TWEEN_TIME)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
        }
    }
    
    private IEnumerator resetObjects(float timeBeforeInactivation)
    {
        yield return new WaitForSeconds(timeBeforeInactivation);
        isOnPauseMenu = false;
        setButtonStatus(false);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        doBlackFadeTween(0.0f, TWEEN_TIME);
        tweenButtons(InOut.OUT);
        StartCoroutine(resetObjects(DELAY_BEFORE_INACTIVATION));
    }
}
