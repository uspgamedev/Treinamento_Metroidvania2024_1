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
        if (playerHealth == null) {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            player = playerHealth.gameObject;
        }

        if (whiteFlashImage == null)
            whiteFlashImage = GameObject.Find("WhiteFade").GetComponent<Image>();

        if (blackBG == null)
            blackBG = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>();

        if (gameOverText == null)
            gameOverText = GameObject.Find("GameOverText").GetComponent<TextMeshProUGUI>();

        if (menuButtons == null)
        {
            menuButtons = GameObject.FindGameObjectsWithTag("GameOverButton");
            System.Array.Sort(menuButtons, (button1, button2) => button2.transform.position.y.CompareTo(button1.transform.position.y));
        }

        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        events = EventSystem.current;
        audioPlayer = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>().GetComponent<SupportScript>().getAudioManagerInstance();

    }

    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        
        setupButtons();
        setButtonStatus(false);
    }

    void Update()
    {
        if (!isGameOver && playerHealth.currentHealth <= 0)
        {
            StartGameOverProcess();
        }

        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                RestartGame();
            else if (Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Backspace))
                ExitGame();
        }
    }

    private void StartGameOverProcess()
    {
        isGameOver = true;
        
        audioPlayer.Pause("LadoA_BGM");
        audioPlayer.Pause("LadoB_BGM");
        audioPlayer.Pause("BossBattle_BGM");
        StartCoroutine(GameOverSequence());
    }

    public void RestartGame()
    {
        if (isGameOver) {
            whiteFlashImage.gameObject.SetActive(true); //Só pra garantir que as outras coisas não quebrem após um gameover
            player.transform.position = support.lastRespawn;
            playerHealth.maxHealth = support.maxHealth;
            playerHealth.HealthRestore(support.maxHealth);
            player.layer = LayerMask.NameToLayer("Player");
            playerHealth.damageable = true;
            playerHealth.toFade = false;

            isGameOver = false;

            ButtonsDisappear();
            events.SetSelectedGameObject(null);

            audioPlayer.Continue("LadoA_BGM");
            audioPlayer.Continue("LadoB_BGM");
            audioPlayer.Continue("BossBattle_BGM");


        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void setButtonStatus(bool status){
        foreach (GameObject button in menuButtons)
        {
            button.SetActive(status);
        }
    }

    private IEnumerator GameOverSequence()
    {
        // White flash
        whiteFlashImage.gameObject.SetActive(true);
        setButtonStatus(true);

        
        blackBG.color = new Color(blackBG.color.r, blackBG.color.g, blackBG.color.b, 1f);
        StartCoroutine(AnimateCharacters());
        AnimateButtons();
        // Slow fade-out to alpha = 0
        float elapsedTime = -1f;
        float fadeOutDuration = 0.5f; // Slow fade-out duration

        while (elapsedTime < fadeOutDuration)
        {
            whiteFlashImage.color = new Color(1, 1, 1, 1 - (elapsedTime / fadeOutDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        whiteFlashImage.color = new Color(1, 1, 1, 0);
        whiteFlashImage.gameObject.SetActive(false);
    }

    private void setupButtons(){
        foreach (GameObject button in menuButtons)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.anchoredPosition += new Vector2(0f, -BUTTON_DISTANCE);
        }
    }

    private void ButtonsDisappear() {
        blackBG.DOFade(0f, 2f);
        setupButtons();
        setButtonStatus(false);
        gameOverText.gameObject.SetActive(false);
    }

    private void AnimateButtons()
    {
        // Agora, inicie a animação movendo os botões para a posição final (para cima)
        float delay = 0f;
        foreach (GameObject button in menuButtons)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + BUTTON_DISTANCE, 0.75f).SetEase(Ease.InOutSine)
                .SetDelay(delay);
            delay = 0.5f;
        }
    }
    private IEnumerator AnimateCharacters()
    {
        yield return new WaitForSeconds(0.2f);
        gameOverText.gameObject.SetActive(true);
        gameOverText.ForceMeshUpdate();
        textInfo = gameOverText.textInfo;

        originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] = new Vector3[textInfo.meshInfo[i].vertices.Length];
            System.Array.Copy(textInfo.meshInfo[i].vertices, originalVertices[i], textInfo.meshInfo[i].vertices.Length);
        }

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;

            Vector3 offset = new Vector3(0, 250, 0);
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }

            gameOverText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return new WaitForSeconds(0.05f);
        }

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
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
    }

    public void ActualQuit() {
        Application.Quit();
    }
}
