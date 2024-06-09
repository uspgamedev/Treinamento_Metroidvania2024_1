using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossDoor : MonoBehaviour
{
    private SupportScript support;

    [Header("Teleport")]
    [SerializeField] private Transform teleportPoint;

    [Header("UI Indicators")]
    [SerializeField] private SpriteRenderer noGemsIndicator;
    [SerializeField] private SpriteRenderer oneGemIndicator;
    [SerializeField] private SpriteRenderer twoGemsIndicator;

    private bool canInteract;

    private Animator animator;
    private Image blackFadeImage;
    private AudioManager audioManager;
    void Start()
    {
        support = GameObject.Find("ScriptsHelper")?.GetComponent<SupportScript>();
        animator = GetComponent<Animator>();
        blackFadeImage = GameObject.FindGameObjectWithTag("BlackFade")?.GetComponent<Image>();
        audioManager = support?.GetAudioManagerInstance();

        if (support == null || animator == null || blackFadeImage == null || audioManager == null)
        {
            Debug.LogError("One or more required components are missing. Please check the GameObject setup.");
        }
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E)) {
            StartCoroutine(TryOpenDoor());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            canInteract = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            canInteract = false;
        }
    }

    private IEnumerator TryOpenDoor() {
        switch (support?.gemCount) {
            case 0:
                yield return FadeIndicator(noGemsIndicator);
                break;
            case 1:
                yield return FadeIndicator(oneGemIndicator);
                break;
            case 2:
            case 3:
                yield return OpenDoor();
                break;
            default:
                Debug.LogWarning("Unexpected gem count: " + support.gemCount);
                break;
        }
    }

    private IEnumerator FadeIndicator(SpriteRenderer indicator)
    {
        indicator.DOFade(1f, 1f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(3f);
        indicator.DOFade(0f, 1f).SetEase(Ease.InOutSine);
    }

    private IEnumerator OpenDoor()
    {
        twoGemsIndicator.DOFade(1f, 0.5f).SetEase(Ease.InOutSine);

        animator.SetTrigger("Open");
        audioManager?.Play("secreto");

        yield return new WaitForSeconds(1f);
        
        blackFadeImage.DOFade(1f, 1f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(0.5f);

        audioManager?.Play("FinalDoor");

        yield return new WaitForSeconds(1.5f);

        twoGemsIndicator.DOFade(0f, 0.5f).SetEase(Ease.InOutSine);
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            player.transform.position = teleportPoint.position;
        }
        blackFadeImage.DOFade(0f, 1f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(2f);
    }
}
