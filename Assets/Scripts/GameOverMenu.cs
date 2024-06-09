using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class GameOverMenu : MonoBehaviour
{
    [HideInInspector] public bool isGameOver = false;
    private const float TWEEN_TIME = 0.3f;
    private const float BUTTON_DISTANCE = 200f;
    private Health playerHealth;
    private Image whiteFlashImage;
    private Image blackBG;
    private TextMeshProUGUI gameOverText;

    private TMP_TextInfo textInfo;
    private Vector3[][] originalVertices;
    private GameObject[] menuButtons;
    private SupportScript support;
    private AudioManager audioPlayer;
    private GameObject player;
    private EventSystem events;

    void Awake()
    {
        InitializeComponents();
        SetupMenuButtons();
    }

    void Start()
    {
        InitializeGameOverMenu();
    }

    void Update()
    {
        if (!isGameOver && playerHealth.currentHealth <= 0)
        {
            StartGameOverProcess();
        }

        HandleInput();
    }

    private void InitializeComponents()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        player = playerHealth.gameObject;

        whiteFlashImage = GameObject.Find("WhiteFade").GetComponent<Image>();
        blackBG = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>();
        gameOverText = GameObject.Find("GameOverText").GetComponent<TextMeshProUGUI>();

        menuButtons = GameObject.FindGameObjectsWithTag("GameOverButton");
        System.Array.Sort(menuButtons, (button1, button2) => button2.transform.position.y.CompareTo(button1.transform.position.y));

        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        events = EventSystem.current;
        audioPlayer = support.GetAudioManagerInstance();
    }

    private void InitializeGameOverMenu()
    {
        gameOverText.gameObject.SetActive(false);
        SetButtonStatus(false);
    }

    private void SetupMenuButtons()
    {
        foreach (GameObject button in menuButtons)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.anchoredPosition += new Vector2(0f, -BUTTON_DISTANCE);
        }
    }

    private void HandleInput()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                RestartGame();
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
                ActualQuit();
        }
    }

    private void StartGameOverProcess()
    {
        isGameOver = true;
        PauseBackgroundMusic();
        StartCoroutine(GameOverSequence());
    }

    private void PauseBackgroundMusic()
    {
        audioPlayer.Pause("LadoA_BGM");
        audioPlayer.Pause("LadoB_BGM");
        audioPlayer.Pause("BossBattle_BGM");
    }

    private void ResumeBackgroundMusic()
    {
        audioPlayer.Continue("LadoA_BGM");
        audioPlayer.Continue("LadoB_BGM");
        audioPlayer.Continue("BossBattle_BGM");
    }

    public void RestartGame()
    {
        if (isGameOver) {
            whiteFlashImage.gameObject.SetActive(true);
            ResetPlayerPosition();
            isGameOver = false;
            StartCoroutine(ButtonsDisappear());
            events.SetSelectedGameObject(null);
            ResumeBackgroundMusic();
        }
    }

    private void ResetPlayerPosition()
    {
        player.transform.position = support.LastRespawn;
        playerHealth.maxHealth = support.maxHealth;
        playerHealth.HealthRestore(support.maxHealth);
        player.layer = LayerMask.NameToLayer("Player");
        playerHealth.damageable = true;
    }

    private void SetButtonStatus(bool status) {
        foreach (GameObject button in menuButtons)
        {
            button.SetActive(status);
        }
    }

    private IEnumerator ButtonsDisappear()
    {
        yield return StartCoroutine(FadeOutGameOverText());
        yield return StartCoroutine(TweenButtonsBack());
        yield return StartCoroutine(FadeOutBlackBG());
        SetButtonStatus(false);
        gameOverText.gameObject.SetActive(false);
    }

    private IEnumerator FadeOutGameOverText()
    {
        gameOverText.DOFade(0f, 1.5f);
        yield return null;
    }

    private IEnumerator TweenButtonsBack()
    {
        float duration = 1f; // Duration of the tween animation

        foreach (GameObject button in menuButtons)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            Vector2 targetPosition = rectTransform.anchoredPosition - new Vector2(0f, BUTTON_DISTANCE);

            // Tween the button back to its original position
            rectTransform.DOAnchorPosY(targetPosition.y, duration).SetEase(Ease.InOutSine);
        }

        // Wait for the tween animation to complete
        yield return new WaitForSeconds(duration);
    }

    private IEnumerator FadeOutBlackBG()
    {
        blackBG.DOFade(0f, 2f);
        yield return null;
    }

    private IEnumerator GameOverSequence()
    {
        StartCoroutine(FadeOutWhiteFlash());
        ShowGameOverUI();
        yield return StartCoroutine(AnimateCharacters());
        AnimateButtons();
    }

    private void ShowGameOverUI()
    {
        gameOverText.gameObject.SetActive(true);
        gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, 1f);
        SetButtonStatus(true);
        blackBG.color = new Color(blackBG.color.r, blackBG.color.g, blackBG.color.b, 1f);
    }

    private IEnumerator FadeOutWhiteFlash()
    {
        
        whiteFlashImage.gameObject.SetActive(true);
        float elapsedTime = -1f; //Tempo negativo pra garantir o atraso de 1 segundo no fade
        float fadeOutDuration = 0.5f;

        while (elapsedTime < fadeOutDuration)
        {
            whiteFlashImage.color = new Color(1, 1, 1, 1 - (elapsedTime / fadeOutDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        whiteFlashImage.color = new Color(1, 1, 1, 0);
        whiteFlashImage.gameObject.SetActive(false);
    }

    private void AnimateButtons()
    {
        float delay = 0f;
        foreach (GameObject button in menuButtons)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + BUTTON_DISTANCE, 0.75f).SetEase(Ease.InOutSine)
                .SetDelay(delay);
            delay += 0.5f;
        }
    }
    private IEnumerator AnimateCharacters()
    {
        yield return new WaitForSeconds(0.2f);
        gameOverText.ForceMeshUpdate();
        textInfo = gameOverText.textInfo;

        SaveOriginalVertices();

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            MoveCharacterUp(i);
            gameOverText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return new WaitForSeconds(0.05f);
        }

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            yield return StartCoroutine(LerpCharacterToOriginalPosition(i));
        }
    }

    private void SaveOriginalVertices()
    {
        originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] = new Vector3[textInfo.meshInfo[i].vertices.Length];
            System.Array.Copy(textInfo.meshInfo[i].vertices, originalVertices[i], textInfo.meshInfo[i].vertices.Length);
        }
    }

    private void MoveCharacterUp(int charIndex)
    {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
        int vertexIndex = charInfo.vertexIndex;
        int materialIndex = charInfo.materialReferenceIndex;

        Vector3 offset = new Vector3(0, 250, 0);
        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

        for (int j = 0; j < 4; j++)
        {
            vertices[vertexIndex + j] += offset;
        }
    }

    private IEnumerator LerpCharacterToOriginalPosition(int charIndex)
    {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
        int vertexIndex = charInfo.vertexIndex;
        int materialIndex = charInfo.materialReferenceIndex;

        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Vector3[] startVertices = new Vector3[4];
        Vector3[] endVertices = new Vector3[4];

        for (int j = 0; j < 4; j++)
        {
            startVertices[j] = vertices[vertexIndex + j];
            endVertices[j] = originalVertices[materialIndex][vertexIndex + j];
        }

        for (float t = 0; t < TWEEN_TIME; t += Time.deltaTime)
        {
            float progress = t / TWEEN_TIME;
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = Vector3.Lerp(startVertices[j], endVertices[j], progress);
            }

            gameOverText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

        for (int j = 0; j < 4; j++)
        {
            vertices[vertexIndex + j] = endVertices[j];
        }

        gameOverText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    public void ActualQuit() {
        Application.Quit();
    }
}
