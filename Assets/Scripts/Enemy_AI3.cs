using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Enemy_AI3 : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;
    private float direction;
    private Transform playerTransform;

    private Transform[] positions;
    private Vector3[] pos;
    private int index = 0;

    private bool cubing = false;
    private bool dying = false;

    private Animator anim;
    private new Light2D light;
    private SimpleFlash flashScript;

    [Header("State Machine")]
    [SerializeField] private float actionTime = 8.5f;
    [SerializeField] private bool onlyTeleports = false;
    [SerializeField] private bool shootsForward = false;
    private float nextChoiceTimer;
    private float baseChoiceMark = 0.5f;
    private float choiceMark;
    private float choice;
    private float minPlayerDist = 35f;

    private Coroutine activeCoroutine;
    private GameObject cubeRange;

    [Header("Hp Drop")]
    [SerializeField] private GameObject hpCollect;
    [Range(0f, 1f)] [SerializeField] private float dropChance;
    
    [SerializeField] private GameObject deathParticles;

    void Awake()
    {   
        int j = 0;
        foreach (Transform child in transform) {
            if (child.GetComponent<Light2D>() == null && child.GetComponent<BlobCubeRange>() == null) {
                j++;
            }
        }
        positions = new Transform[j];
        j = 0;
        foreach (Transform child in transform) {
            if (child.GetComponent<Light2D>() == null && child.GetComponent<BlobCubeRange>() == null) {
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
        flashScript = GetComponent<SimpleFlash>();

        cubeRange = transform.Find("Cube Range").gameObject;

        if (pos.Length <= 1) {
            baseChoiceMark = 0f;
        }

        nextChoiceTimer = actionTime;
        choiceMark = baseChoiceMark;

        if (shootsForward) {
            minPlayerDist *= 2;
        }
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
            float randomDeviation = Random.Range(-2f, 2f);
            if (shootsForward) {
                randomDeviation = 0f;
            }
            nextChoiceTimer = actionTime + randomDeviation;

            if (PlayerClose()) {

                if (onlyTeleports) {
                    StartNewCoroutine(TrocarPosicao());
                }

                else {
                    choice = Random.Range(0f, 1f);
                    if (choice >= choiceMark) {
                        StartNewCoroutine(Shoot());
                        choiceMark = baseChoiceMark;
                    }
                    else {
                        StartNewCoroutine(TrocarPosicao());
                        choiceMark /= 2;
                    }
                }
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
        disparo.shotForward = shootsForward;

        activeCoroutine = null;
    }

    public void StartNewCoroutine(IEnumerator coroutine)
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

    private void OnTeleport(bool state) {
        GetComponent<SpriteRenderer>().enabled = state;
        GetComponent<BoxCollider2D>().enabled = state;
        // GetComponent<CircleCollider2D>().enabled = state;
        cubeRange.SetActive(state);
        if (!state) {
            GetComponent<Rigidbody2D>().gravityScale = 0f;
        }
        else {
            GetComponent<Rigidbody2D>().gravityScale = 1f;
        }
    }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.tag == "Player"){
    //         anim.SetTrigger("Cube");
    //         cubing = true;
    //         if (!dying) {
    //             StartNewCoroutine(null);
    //         }
    //         if (light.intensity > 0f) {
    //             light.intensity = 0f;
    //         }
    //     }
    // }

    // void OnTriggerExit2D(Collider2D collision){
    //     if (collision.gameObject.tag == "Player"){
    //         anim.SetTrigger("Decube");
    //         cubing = false;
    //     }
    // }

    public IEnumerator Die() {
        dying = true;
        flashScript.Flash(Color.green);

        float willDrop = Random.Range(0f, 1f);

        if (willDrop <= dropChance) {
            GameObject hp = Instantiate(hpCollect, transform.position + new Vector3(0f, 0f, 0f), Quaternion.identity);
            hp.GetComponent<HealthRecover>().goRight = GameObject.Find("Player").transform.position.x < transform.position.x;
        }

        GameObject part = Instantiate(deathParticles, transform.position, Quaternion.identity);
        part.GetComponent<DeathParticles>().enemy = DeathParticles.Enemy.Blob;

        yield return new WaitForSeconds(0.125f);

        gameObject.tag = "Morto";
        dying = false;
        gameObject.SetActive(false);
    }

    private bool PlayerClose() {
        Vector3 vector = playerTransform.position - transform.position;
        float magnitude = vector.magnitude;
        return magnitude < minPlayerDist;
    }

    public void Cube() {
        anim.SetTrigger("Cube");
        cubing = true;
        if (!dying) {
            StartNewCoroutine(null);
        }
        if (light.intensity > 0f) {
            light.intensity = 0f;
        }
    }

    public void Decube() {
        anim.SetTrigger("Decube");
        cubing = false;
    }
}
