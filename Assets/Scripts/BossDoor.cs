using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossDoor : MonoBehaviour
{
    private SupportScript support;

    [Header("Teleport")]
    [SerializeField] private Transform tpPoint;

    [Header("UI Indicators")]
    [SerializeField] private SpriteRenderer noGems;
    [SerializeField] private SpriteRenderer oneGem;
    [SerializeField] private SpriteRenderer twoGems;

    private bool canPickup;

    private Animator anim;

    private Image blackFade;
    private AudioManager audioPlayer;
    void Start()
    {
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        anim = GetComponent<Animator>();
        blackFade = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>();
        audioPlayer = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>().getAudioManagerInstance();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPickup) {
            StartCoroutine(TryOpen());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") {
            canPickup = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player") {
            canPickup = false;
        }
    }

    private IEnumerator TryOpen() {
        switch (support.gemCount) {
            case 0:
                noGems.DOFade(1f, 1f).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(3f);
                noGems.DOFade(0f, 1f).SetEase(Ease.InOutSine);
                break;
            case 1:
                oneGem.DOFade(1f, 1f).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(3f);
                oneGem.DOFade(0f, 1f).SetEase(Ease.InOutSine);
                break;
            case 2: //Não havia reparado nisso, mas basicamente esse coiso impede o mano de fazer a fuga.
            case 3:
                twoGems.DOFade(1f, 1f).SetEase(Ease.InOutSine);
                StartCoroutine(Open());
                yield return new WaitForSeconds(1f);
                twoGems.DOFade(0f, 1f).SetEase(Ease.InOutSine);
                break;
        }
    }

    private IEnumerator Open() {
        
        anim.SetTrigger("Open");

        audioPlayer.Play("secreto");

        yield return new WaitForSeconds(1.5f);
        
        blackFade.DOFade(1f, 2f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(0.5f);

        audioPlayer.Play("FinalDoor");

        yield return new WaitForSeconds(1.5f);
        
        GameObject.Find("Player").transform.position = tpPoint.position;
        blackFade.DOFade(0f, 2f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(2f);
    }
}
