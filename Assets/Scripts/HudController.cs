using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.Linq;

public class HudController : MonoBehaviour
{
    private enum InOut { IN, OUT }

    private const float TWEEN_TIME = 0.5f;
    private const float BLACK_FADE_ALPHA_IN = 0.4f;
    private const float BLACK_FADE_ALPHA_OUT = 1f;
    private const float DELAY_BEFORE_RETURN = 0.4f;
    private const float DELAY_BEFORE_INACTIVATION = 0.6f;

    private Health playerHealth;
    private GameObject[] menuButtons;
    private Image blackFade;
    private GameObject pauseText;
    private SupportScript support;
    private GameObject player;
    private bool isOnPauseMenu = false;
    private bool isTweening = false; // Flag to check if tweening is in progress
    private GameObject gameOverMenu;
    private AudioManager audioPlayer;

    private void Awake()
    {
        InitializeComponents();
        InitializePlayerHealth();
        InitializeSupportScript();
        InitializeGameOverMenu();
        SetButtonStatus(false);
    }

    private void InitializeComponents()
    {
        blackFade = GameObject.Find("BlackFade").GetComponent<Image>();
        pauseText = GameObject.Find("Pause (text)");
        menuButtons = GameObject.FindGameObjectsWithTag("MenuButton")
                                .OrderByDescending(button => button.transform.position.y)
                                .ToArray();
    }

    private void InitializePlayerHealth()
    {
        if (playerHealth == null)
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            player = playerHealth.gameObject;
        }
    }

    private void InitializeSupportScript()
    {
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
    }

    private void InitializeGameOverMenu()
    {
        gameOverMenu = GameObject.Find("GameOverMenu");
    }

    private void Start()
    {
        audioPlayer = support.getAudioManagerInstance();
        TweenButtons(InOut.OUT);
    }

    private void Update()
    {
        if (!gameOverMenu.GetComponent<GameOverMenu>().isGameOver && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.P)))
        {
            if (isTweening) return; // Prevent actions during tweening

            if (isOnPauseMenu)
            {
                ContinueGame();
            }
            else
            {
                CallPauseMenu();
            }
        }
    }

    private void CallPauseMenu()
    {
        audioPlayer.Play("PauseEnter");
        PauseBackgroundMusic();
        isOnPauseMenu = true;
        SetButtonStatus(true);
        Time.timeScale = 0;
        isTweening = true; // Set tweening flag

        PerformBlackFade(BLACK_FADE_ALPHA_IN, TWEEN_TIME, () => 
        {
            isTweening = false; // Reset tweening flag after tween is complete
        });

        TweenButtons(InOut.IN);
    }

    private void PauseBackgroundMusic()
    {
        audioPlayer.Pause("LadoB_BGM");
        audioPlayer.Pause("LadoA_BGM");
        audioPlayer.Pause("BossBattle_BGM");
    }

    private void ContinueGame()
    {
        if (isTweening) return; // Prevent actions during tweening

        audioPlayer.Continue("LadoB_BGM");
        audioPlayer.Continue("LadoA_BGM");
        audioPlayer.Continue("BossBattle_BGM");
        Time.timeScale = 1;
        isTweening = true; // Set tweening flag

        PerformBlackFade(0.0f, TWEEN_TIME, () => 
        {
            isTweening = false; // Reset tweening flag after tween is complete
        });

        TweenButtons(InOut.OUT);
        StartCoroutine(ResetObjects(DELAY_BEFORE_INACTIVATION));
    }

    private void PerformBlackFade(float targetAlpha, float duration, TweenCallback onComplete = null)
    {
        blackFade.DOFade(targetAlpha, duration).SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(onComplete);
    }

    private IEnumerator CallToReturn(float delay)
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(delay);
        SetButtonStatus(false);
        ReturnToMenu();
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void SetButtonStatus(bool status)
    {
        pauseText.SetActive(status);
        foreach (var button in menuButtons)
        {
            button.SetActive(status);
        }
    }

    private void TweenButtons(InOut direction)
    {
        TextMeshProUGUI pauseTextMeshPro = pauseText.GetComponent<TextMeshProUGUI>();
        Color targetColor = pauseTextMeshPro.color;
        targetColor.a = direction == InOut.IN ? 1f : 0f;
        pauseTextMeshPro.DOKill();
        pauseTextMeshPro.DOColor(targetColor, TWEEN_TIME).SetUpdate(true);

        foreach (var button in menuButtons)
        {
            var rectTransform = button.GetComponent<RectTransform>();
            rectTransform.DOKill();

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

    private IEnumerator ResetObjects(float delay)
    {
        yield return new WaitForSeconds(delay);
        isOnPauseMenu = false;
        SetButtonStatus(false);
    }

    public void RestartFromCheckpoint()
    {
        StartCoroutine(CheckpointReturn());
    }

    private IEnumerator CheckpointReturn()
    {
        Time.timeScale = 1;
        isTweening = true; // Set tweening flag

        PerformBlackFade(1f, TWEEN_TIME, () => 
        {
            isTweening = false; // Reset tweening flag after tween is complete
        });

        yield return new WaitForSeconds(TWEEN_TIME);
        player.transform.position = support.lastRespawn;
        playerHealth.maxHealth = support.maxHealth;
        playerHealth.HealthRestore(support.maxHealth);
        player.layer = LayerMask.NameToLayer("Player");
        playerHealth.damageable = true;
        ContinueGame();
    }

    public void ActualQuit()
    {
        Application.Quit();
    }
}
