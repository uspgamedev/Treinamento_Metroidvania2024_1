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
    private float nextFireTime;
    [SerializeField] private float fireRate = 1f;

    [SerializeField] private Transform[] positions;
    private Vector3[] pos;
    private int k = 0;

    private bool cubing = false;
    private bool shooting = false;
    private bool changing = false;
    private bool willChange = false;
    private float timeSinceChange = 0f;
    private bool changingRunning = false;
    private bool shootRunning = false;

    private Animator anim;
    private new Light2D light;

    void Start()
    {   
        pos = new Vector3[positions.Length];
        for (int i = 0; i < positions.Length; i++) {
            pos[i] = transform.position + positions[i].localPosition;
        }

        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        light = transform.GetChild(0).GetComponent<Light2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(TrocarPosicao());

        nextFireTime = 1f/fireRate;
    }

    void Update()
    {
        if (playerTransform.position.x > transform.position.x){
            direction = 1f;
        } else {
            direction = -1f;
        }

        nextFireTime -= Time.deltaTime;
        timeSinceChange += Time.deltaTime;

        if (nextFireTime < 0f && !shooting && !changing) {
            StartCoroutine(Shoot());
        }

        if (timeSinceChange > 25f && !shooting && !changing && !cubing && !changingRunning) {
            StartCoroutine(TrocarPosicao());
        }
        Debug.Log("changing: " + changing);
        Debug.Log("shooting: " + shooting);
        Debug.Log("cubing: " + cubing);
        // if (willChange && !shooting && !changing && !cubing) {
        //     willChange = false;
        //     StartCoroutine(TrocarPosicao());
        // }
    }

    private IEnumerator TrocarPosicao() {
        changingRunning = true;

        if (pos.Length > 0) {

            yield return new WaitForSeconds(1.4f);

            changing = false;

            yield return new WaitForSeconds(Random.Range(5, 15));


            if (pos.Length > 0 && !shooting) {
                anim.SetTrigger("Teleport");
                timeSinceChange = 0f;
                changing = true;

                if (k<pos.Length-1){
                    k++;
                } else {
                    k=0;
                }

                Debug.Log("Tp");

                yield return new WaitForSeconds(1.4f);
                OnTeleport(false);

                yield return new WaitForSeconds(2.5f);
                OnTeleport(true);

                anim.SetTrigger("Appear");

                transform.position = pos[k];

                if (!cubing){
                    StartCoroutine(TrocarPosicao());
                }
                if (shooting) {
                    willChange = true;
                }
            }
            else {
                willChange = true;
                changing = false;
            }
        }
        changingRunning = false;
    }

    private IEnumerator Shoot()
    {
        shootRunning = true;

        nextFireTime = 1f/fireRate;
        shooting = true;

        if (!cubing && !changing) {

            Debug.Log("Tiro");
            
            anim.SetTrigger("Prepare");
            light.gameObject.SetActive(true);
            light.intensity = 0f;
            DOTween.To(() => light.intensity, (x) => light.intensity = x, 1.6f, 1f);

            yield return new WaitForSeconds(2f);

            light.intensity = 0f;
            light.gameObject.SetActive(false);

            if (!cubing) {
                anim.SetTrigger("Attack");
                GameObject projectile = Instantiate(projectilePrefab, new Vector2(transform.position.x + direction, transform.position.y), Quaternion.identity);
                Disparo disparo = projectile.GetComponent<Disparo>();
                disparo.projectileSpeed = projectileSpeed;
            }
        }
        shooting = false;

        shootRunning = false;
    }

    private void OnTeleport(bool to) {
        GetComponent<SpriteRenderer>().enabled = to;
        GetComponent<BoxCollider2D>().enabled = to;
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
            StopCoroutine(TrocarPosicao());
            changing = false;
            changingRunning = false;
        }
    }

    void OnTriggerExit2D(Collider2D collision){
        if (collision.gameObject.tag == "Player"){
            anim.SetTrigger("Decube");
            cubing = false;
            shooting = false;
        }
    }
}