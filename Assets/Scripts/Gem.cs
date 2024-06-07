using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Gem : MonoBehaviour
{
    private SupportScript support;

    private bool canPickup = false;
    private SimpleFlash flashScript;

    [SerializeField] private Color color;

    [SerializeField] private int gemID;

    [SerializeField] private new Light2D light;

    [SerializeField] private bool finalGem = false;

    [SerializeField] private GameObject colliderFinal;

    void Start()
    {
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        flashScript = GetComponent<SimpleFlash>();

        foreach (int id in support.gemIDToDeactivate) {
            if (id == gemID) {
                gameObject.SetActive(false);
            }
        }

        // if (finalGem) {
        //     colliderFinal = GameObject.FindObjectOfType<SequenciaFinal>(true).gameObject;
        // }
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPickup) {
            StartCoroutine(GetGem());
            canPickup = false;
        }
    }

    private IEnumerator GetGem()
    {
        flashScript.Flash(color);
        GameObject.Find("Player").GetComponent<SimpleFlash>().Flash(color);

        if (finalGem) {
            colliderFinal.SetActive(true);
        }

        support.gemCount++;
        support.gemIDToDeactivate.Add(gemID);

        DOTween.To(() => light.intensity, (x) => light.intensity = x, light.intensity + 1.5f, 1f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(flashScript.duration);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;

        yield return new WaitForSeconds(1f - flashScript.duration);

        DOTween.To(() => light.intensity, (x) => light.intensity = x, 0f, 2.5f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}