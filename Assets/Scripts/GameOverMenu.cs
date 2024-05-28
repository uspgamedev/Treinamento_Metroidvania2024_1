using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [HideInInspector] public bool isGameOver = false;
    private Health playerHealth;
    private CinemachineVirtualCamera cinemachineCamera;
    private Image whiteFlashImage; 
    private Image blackBG;
    private Image floorCircle;
    public float zoomDuration = 2.0f;

    void Awake()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        cinemachineCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>();
        whiteFlashImage = GameObject.Find("WhiteFade").GetComponent<Image>();
        blackBG = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>();
        floorCircle = GameObject.Find("ProjectedFloor").GetComponent<Image>();
    }

    void Start(){
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
}
