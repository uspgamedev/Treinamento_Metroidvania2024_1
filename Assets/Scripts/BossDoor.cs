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

    void Start()
    {
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        anim = GetComponent<Animator>();
        blackFade = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>();
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
            case 2:
                twoGems.DOFade(1f, 1f).SetEase(Ease.InOutSine);
                StartCoroutine(Open());
                yield return new WaitForSeconds(1f);
                twoGems.DOFade(0f, 1f).SetEase(Ease.InOutSine);
                break;
        }
    }

    private IEnumerator Open() {
        
        anim.SetTrigger("Open");

        yield return new WaitForSeconds(1.5f);
        
        blackFade.DOFade(1f, 2f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(2f);
        
        GameObject.Find("Player").transform.position = tpPoint.position;
        blackFade.DOFade(0f, 2f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(2f);
    }
}
