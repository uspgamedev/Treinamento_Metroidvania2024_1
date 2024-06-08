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
    private const float TWEEN_TIME = 0.5f;
    private Health playerHealth;
    private const float BLACK_FADE_ALPHA_IN = 0.4f;
    private const float BLACK_FADE_ALPHA_OUT = 1f;
    private const float DELAY_BEFORE_RETURN = 0.4f;
    private const float DELAY_BEFORE_INACTIVATION = 0.6f;
    private GameObject[] menuButtons;
    private Image blackFade;
    private GameObject pauseText;
    private SupportScript support;
    private GameObject player;
    private Tween currentTween; // Variável para manter uma referência ao tween atual
    [HideInInspector] public bool isOnPauseMenu = false;
    private GameObject gameOverMenu;
    private AudioManager audioPlayer;

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

        if (playerHealth == null) {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            player = playerHealth.gameObject;
        }
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();

        gameOverMenu = GameObject.Find("GameOverMenu");
        setButtonStatus(false);
    }

    void Start(){
        audioPlayer = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>().GetComponent<SupportScript>().getAudioManagerInstance();
        tweenButtons(InOut.OUT);
    }

    void Update()
    {
        if (!gameOverMenu.GetComponent<GameOverMenu>().isGameOver && Input.GetKeyDown(KeyCode.Escape))
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
        audioPlayer.Play("PauseEnter");
        audioPlayer.Pause("LadoB_BGM");
        audioPlayer.Pause("LadoA_BGM");
        audioPlayer.Pause("BossBattle_BGM");

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
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        doBlackFadeTween(BLACK_FADE_ALPHA_OUT, TWEEN_TIME);
        tweenButtons(InOut.OUT);
        StartCoroutine(callToReturn(DELAY_BEFORE_RETURN));
        // StartCoroutine(resetObjects(DELAY_BEFORE_RETURN));
    }

    private IEnumerator callToReturn(float timeBeforeReturn)
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(timeBeforeReturn);
        setButtonStatus(false);
        ReturnToMenu();
    }

    private void tweenButtons(InOut direction)
    {
        TextMeshProUGUI pauseTextMeshPro = pauseText.GetComponent<TextMeshProUGUI>();
        Color targetColor = pauseTextMeshPro.color;
        targetColor.a = direction == InOut.IN ? 1f : 0f;

        pauseTextMeshPro.DOKill(); // Cancelar o tween anterior, se existir

        pauseTextMeshPro.DOColor(targetColor, TWEEN_TIME).SetUpdate(true);

        int numButtons = menuButtons.Length;

        for (int i = 0; i < numButtons; i++)
        {
            var rectTransform = menuButtons[i].GetComponent<RectTransform>();
            rectTransform.DOKill(); // Cancelar o tween anterior, se existir

            Vector2 targetAnchorMin = rectTransform.anchorMin;
            Vector2 targetAnchorMax = rectTransform.anchorMax;

            if (direction == InOut.IN)
            {
                targetAnchorMin.y = 0.25f; 
                targetAnchorMax.y = 0.25f; 
            }
            else
            {
                targetAnchorMin.y = -1.5f;
                targetAnchorMax.y = -1.5f; 
            }

            rectTransform.DOAnchorMin(targetAnchorMin, TWEEN_TIME)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);

            rectTransform.DOAnchorMax(targetAnchorMax, TWEEN_TIME)
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

    public void RestartFromCheckpoint() //Só para o caso de alguma coisa dar errado e cairmos num softlock
    {                                   //Acredito ter corrigido e / ou ter dado oportunidade de fuga de todos os softlocks mas só pra garantir né
        StartCoroutine(checkpointReturn());

    }
    private IEnumerator checkpointReturn(){
        Time.timeScale = 1;
        doBlackFadeTween(1f, TWEEN_TIME);
        yield return new WaitForSeconds(TWEEN_TIME);
        player.transform.position = support.lastRespawn;
        playerHealth.maxHealth = support.maxHealth;
        playerHealth.HealthRestore(support.maxHealth);
        player.layer = LayerMask.NameToLayer("Player");
        playerHealth.damageable = true;
        playerHealth.toFade = false;
        StartCoroutine(desFade());
    }
    private IEnumerator desFade(){
        
        tweenButtons(InOut.OUT);
        StartCoroutine(resetObjects(DELAY_BEFORE_INACTIVATION));
        yield return new WaitForSeconds(TWEEN_TIME);
        
        doBlackFadeTween(0f, TWEEN_TIME);
        audioPlayer.Continue("LadoA_BGM");
        audioPlayer.Continue("LadoB_BGM");
        audioPlayer.Continue("BossBattle_BGM");
        yield return new WaitForSeconds(TWEEN_TIME);
    }

    public void ContinueGame()
    {
        audioPlayer.Continue("LadoB_BGM");
        audioPlayer.Continue("LadoA_BGM");
        audioPlayer.Continue("BossBattle_BGM");
        Time.timeScale = 1;
        doBlackFadeTween(0.0f, TWEEN_TIME);
        tweenButtons(InOut.OUT);
        StartCoroutine(resetObjects(DELAY_BEFORE_INACTIVATION));
    }

    public void ActualQuit() {
        Application.Quit();
    }
}
