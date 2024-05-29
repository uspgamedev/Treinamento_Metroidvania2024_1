using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private float fallDuration = 1.0f;
    [SerializeField] private float bounceHeight = 1.0f;
    [SerializeField] private float delayBetweenLetters = 0.1f;
    [HideInInspector] public bool isGameOver = false;
    private Health playerHealth;
    private Image whiteFlashImage; 
    private Image blackBG;
    private Image floorCircle;
    private TMP_Text gameOverText;

    void Awake()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        whiteFlashImage = GameObject.Find("WhiteFade").GetComponent<Image>();
        blackBG = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>();
        floorCircle = GameObject.Find("ProjectedFloor").GetComponent<Image>();
        gameOverText = GameObject.Find("GameOverText").GetComponent<TMP_Text>();
    }

    void Start(){
        gameOverText.gameObject.SetActive(false);
        floorCircle.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isGameOver && playerHealth.currentHealth <= 0)
        {
            startGameOverProcess();
        }
        if (isGameOver){
            if (Input.GetKeyDown(KeyCode.Return)){ //O enter do teclado é entendido como Return na Unity.
                restartGame(); 
            }
        }
    }

    private void startGameOverProcess()
    {
        isGameOver = true;
        StartCoroutine(GameOverSequence());
    }

    private void restartGame(){
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private IEnumerator GameOverSequence()
    {

        // White flash
        whiteFlashImage.gameObject.SetActive(true);
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

        StartCoroutine(BounceLetters());
        floorCircle.gameObject.SetActive(true);
        blackBG.color = new Color(blackBG.color.r, blackBG.color.g, blackBG.color.b, 1f);
        // Wait for half a second
        yield return new WaitForSeconds(0.5f);

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

    IEnumerator BounceLetters()
    {
        
        gameOverText.gameObject.SetActive(true);
        gameOverText.ForceMeshUpdate();
        TMP_TextInfo textInfo = gameOverText.textInfo;
        
        Vector3[] initialPositions = new Vector3[textInfo.characterCount];
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            initialPositions[i] = textInfo.characterInfo[i].bottomLeft;
        }
        
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            StartCoroutine(BounceLetter(i, initialPositions[i]));
            yield return new WaitForSeconds(delayBetweenLetters);
        }
    }

    IEnumerator BounceLetter(int index, Vector3 initialPosition)
    {
        TMP_TextInfo textInfo = gameOverText.textInfo;
        Vector3 startPos = initialPosition + Vector3.up * bounceHeight;
        Vector3 endPos = initialPosition;
        
        float timeElapsed = 0;
        
        while (timeElapsed < fallDuration)
        {
            float t = timeElapsed / fallDuration;
            Vector3 currentPosition = Vector3.Lerp(startPos, endPos, t) + Vector3.up * bounceHeight;

            int meshIndex = textInfo.characterInfo[index].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[index].vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            vertices[vertexIndex + 0] = currentPosition;
            vertices[vertexIndex + 1] = currentPosition;
            vertices[vertexIndex + 2] = currentPosition;
            vertices[vertexIndex + 3] = currentPosition;

            gameOverText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
