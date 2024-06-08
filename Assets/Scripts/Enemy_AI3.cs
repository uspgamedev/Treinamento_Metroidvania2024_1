using System.Collections;
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
    private bool teleporting = false;
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
    private float minPlayerDistX = 18f;
    private float minPlayerDistY = 8f;

    private Coroutine activeCoroutine;
    private GameObject cubeRange;

    [Header("Hp Drop")]
    [SerializeField] private GameObject hpCollect;
    [Range(0f, 1f)] [SerializeField] private float dropChance;

    [SerializeField] private GameObject deathParticles;

    private SupportScript support;
    private AudioManager audioPlayer;

    [SerializeField] private const float respawnTime = 150f;

    private void Awake()
    {
        InitializeComponents();
        InitializePositions();
        InitializeTimers();
    }

    private void InitializeComponents()
    {
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        audioPlayer = support.GetComponent<SupportScript>().getAudioManagerInstance();

        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        light = transform.GetChild(0).GetComponent<Light2D>();
        anim = GetComponent<Animator>();
        flashScript = GetComponent<SimpleFlash>();
        cubeRange = transform.Find("Cube Range").gameObject;
    }

    private void InitializePositions()
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

        if (pos.Length <= 1) {
            baseChoiceMark = 0f;
        }
    }

    private void InitializeTimers()
    {
        nextChoiceTimer = actionTime;
        choiceMark = baseChoiceMark;

        if (shootsForward) {
            minPlayerDistX *= 2;
        }
    }

    void Update()
    {
        UpdateDirection();
        UpdateChoiceTimer();
    }

    private void UpdateDirection()
    {
        if (playerTransform.position.x > transform.position.x){
            direction = 1f;
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            direction = -1f;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private void UpdateChoiceTimer()
    {
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
        audioPlayer.Play("BlobDisappear");

        index += Random.Range(1, pos.Length);
        if (index >= pos.Length) {
            index -= pos.Length;
        }

        teleporting = true;

        yield return new WaitForSeconds(1.4f);
        OnTeleport(false);

        yield return new WaitForSeconds(2.5f);
        OnTeleport(true);
        teleporting = false;
        anim.SetTrigger("Appear");
        audioPlayer.Play("BlobAppear");

        transform.position = pos[index];

        activeCoroutine = null;
    }

    private IEnumerator Shoot()
    {
        audioPlayer.Play("BlobPreShoot");
        anim.SetTrigger("Prepare");
        light.gameObject.SetActive(true);
        light.intensity = 0f;
        DOTween.To(() => light.intensity, (x) => light.intensity = x, 1.6f, 1f);

        yield return new WaitForSeconds(2f);

        light.intensity = 0f;
        light.gameObject.SetActive(false);

        anim.SetTrigger("Attack");
        audioPlayer.Play("BlobShoot");
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
        cubeRange.SetActive(state);
        GetComponent<Rigidbody2D>().gravityScale = state ? 1f : 0f;
    }

    public IEnumerator Die() {
        dying = true;
        flashScript.Flash(Color.green);

        if (Random.Range(0f, 1f) <= dropChance) {
            GameObject hp = Instantiate(hpCollect, transform.position, Quaternion.identity);
            hp.GetComponent<HealthRecover>().goRight = playerTransform.position.x < transform.position.x;
        }

        GameObject part = Instantiate(deathParticles, transform.position, Quaternion.identity);
        audioPlayer.Play("BlobDie");
        part.GetComponent<DeathParticles>().enemy = DeathParticles.Enemy.Blob;

        yield return new WaitForSeconds(0.125f);

        support.StartEnemyRespawn(gameObject, respawnTime, gameObject.tag);

        gameObject.tag = "Morto";
        dying = false;
        gameObject.SetActive(false);
    }

    private bool PlayerClose() {
        float x = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float y = Mathf.Abs(playerTransform.position.y - transform.position.y);
        // Math.Abs pra garantir que os bixos não continuem atirando caso o player esteja muito a esquerda, ou muito abaixo dos bixos
        // Vetores e Geometria Feat: Colucci moment
        return x < minPlayerDistX && y < minPlayerDistY;
    }

    public void Cube() {
        if (!teleporting) {
            //Debug.Log("a");
            anim.SetTrigger("Cube");
            cubing = true;
            if (!dying) {
                StartNewCoroutine(null);
            }
            if (light.intensity > 0f) {
                light.intensity = 0f;
            }
        }
    }

    public void Decube() {
        anim.SetTrigger("Decube");
        cubing = false;
    }
}
