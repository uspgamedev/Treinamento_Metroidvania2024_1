using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Enemy_AI3 : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;
    private float direction;
    private Transform playerTransform;

    private Transform[] positions;
    private Vector3[] pos;
    private int index = 0;

    private bool cubing = false;

    private Animator anim;
    private new Light2D light;

    [SerializeField] private float actionTime = 8.5f;
    private float nextChoiceTimer;
    private float baseChoiceMark = 0.5f;
    private float choiceMark;
    private float choice;

    private Coroutine activeCoroutine;

    void Start()
    {   
        int j = 0;
        foreach (Transform child in transform) {
            if (child.GetComponent<Light2D>() == null) {
                j++;
            }
        }
        positions = new Transform[j];
        j = 0;
        foreach (Transform child in transform) {
            if (child.GetComponent<Light2D>() == null) {
                positions[j] = child.transform;
                j++;
            }
        }
        pos = new Vector3[positions.Length + 1];
        pos[0] = transform.position;

        for (int i = 0; i < positions.Length; i++) {
            pos[i+1] = transform.position + positions[i].localPosition;
        }

        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        light = transform.GetChild(0).GetComponent<Light2D>();
        anim = GetComponent<Animator>();

        nextChoiceTimer = actionTime;
        choiceMark = baseChoiceMark;
    }

    void Update()
    {
        if (playerTransform.position.x > transform.position.x){
            direction = 1f;
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            direction = -1f;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        nextChoiceTimer -= Time.deltaTime;
        if (cubing) {
            nextChoiceTimer = actionTime * 0.6f;
        }

        if (nextChoiceTimer < 0f) {
            nextChoiceTimer = Random.Range(actionTime - 2f, actionTime + 2f);
            choice = Random.Range(0f, 1f);

            if (choice > choiceMark) {
                StartNewCoroutine(Shoot());
                choiceMark = baseChoiceMark;
            }
            else {
                StartNewCoroutine(TrocarPosicao());
                choiceMark /= 2;
            }
        }
    }

    private IEnumerator TrocarPosicao() {
        anim.SetTrigger("Teleport");

        index += Random.Range(1, pos.Length);
        if (index >= pos.Length) {
            index -= pos.Length;
        }

        yield return new WaitForSeconds(1.4f);
        OnTeleport(false);

        yield return new WaitForSeconds(2.5f);
        OnTeleport(true);
        anim.SetTrigger("Appear");

        transform.position = pos[index];

        activeCoroutine = null;
    }

    private IEnumerator Shoot()
    {
        anim.SetTrigger("Prepare");
        light.gameObject.SetActive(true);
        light.intensity = 0f;
        DOTween.To(() => light.intensity, (x) => light.intensity = x, 1.6f, 1f);

        yield return new WaitForSeconds(2f);

        light.intensity = 0f;
        light.gameObject.SetActive(false);

        anim.SetTrigger("Attack");
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(transform.position.x + direction, transform.position.y), Quaternion.identity);
        Disparo disparo = projectile.GetComponent<Disparo>();
        disparo.projectileSpeed = projectileSpeed;

        activeCoroutine = null;
    }

    private void StartNewCoroutine(IEnumerator coroutine)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        if (coroutine != null)
        {
            activeCoroutine = StartCoroutine(coroutine);
        }
    }

    private void OnTeleport(bool to) {
        GetComponent<SpriteRenderer>().enabled = to;
        GetComponent<BoxCollider2D>().enabled = to;
        GetComponent<CircleCollider2D>().enabled = to;
        if (!to) {
            GetComponent<Rigidbody2D>().gravityScale = 0f;
        }
        else {
            GetComponent<Rigidbody2D>().gravityScale = 1f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"){
            anim.SetTrigger("Cube");
            cubing = true;
            StartNewCoroutine(null); // Stop any active coroutine
        }
    }

    void OnTriggerExit2D(Collider2D collision){
        if (collision.gameObject.tag == "Player"){
            anim.SetTrigger("Decube");
            cubing = false;
        }
    }
}
