using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    private GameObject player;
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
    }

    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        setupButtons();
        setButtonStatus(true);
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
            else if (Input.GetKeyDown(KeyCode.Escape))
                ExitGame();
        }
    }

    private void StartGameOverProcess()
    {
        isGameOver = true;
        StartCoroutine(GameOverSequence());
    }

    public void RestartGame()
    {
        player.transform.position = support.lastRespawn;
        playerHealth.maxHealth = support.maxHealth;
        playerHealth.HealthRestore(support.maxHealth);

        // Scene scene = SceneManager.GetActiveScene();
        // SceneManager.LoadScene(scene.name);
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
        whiteFlashImage.color = new Color(1, 1, 1, 0);
        float elapsedTime = 0f;
        float fadeInDuration = 0.1f; // Fast fade-in duration
        // Fast fade-in to alpha = 1
        while (elapsedTime < fadeInDuration)
        {
            whiteFlashImage.color = new Color(1, 1, 1, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        whiteFlashImage.color = new Color(1, 1, 1, 1);

        
        blackBG.color = new Color(blackBG.color.r, blackBG.color.g, blackBG.color.b, 1f);
        // Wait for half a second
        StartCoroutine(AnimateCharacters());
        yield return new WaitForSeconds(0.5f);
        AnimateButtons();
        // Slow fade-out to alpha = 0
        elapsedTime = 0f;
        float fadeOutDuration = 1.0f; // Slow fade-out duration

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
}
