using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Enemy_AI3 : MonoBehaviour
{

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    private float direction;
    [SerializeField] private Transform playerTransform;
    private float nextFireTime;
    [SerializeField] private float fireRate = 1f;

    private float timer =0f;

    [SerializeField] private Transform[] positions;
    private Vector3[] pos;
    private int k=0;

    private bool cubing = false;
    private bool shooting = false;
    private bool changing = false;
    private bool willChange = false;

    private Animator anim;

    void Start()
    {   
        pos = new Vector3[positions.Length];
        for (int i = 0; i < positions.Length; i++) {
            pos[i] = transform.position + positions[i].localPosition;
        }
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        anim = GetComponent<Animator>();
        StartCoroutine(TrocarPosicao());

        Debug.Log(pos.Length);
    }

    void Update()
    {

        if (playerTransform.position.x > transform.position.x){
            direction = 1f;
        } else {
            direction = -1f;
        }

        nextFireTime -= Time.deltaTime;
        timer -= Time.deltaTime;

        if (nextFireTime < 0f && !shooting && !changing) {
            StartCoroutine(Shoot());
        }

        if (willChange && !shooting && !changing) {
            willChange = false;
            StartCoroutine(TrocarPosicao());
        }
    }

    private IEnumerator TrocarPosicao() {

        yield return new WaitForSeconds(1.4f);

        changing = false;

        yield return new WaitForSeconds(Random.Range(5, 15));
        
        changing = true;

        Debug.Log(pos.Length);

        if (pos.Length > 0 && !shooting) {
            anim.SetTrigger("Teleport");
            
            if (k<pos.Length-1){
                k++;
            } else {
                k=0;
            }

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
    }

    private IEnumerator Shoot()
    {
        nextFireTime = 1f/fireRate;
        shooting = true;

        if (!cubing && !changing) {
            anim.SetTrigger("Prepare");

            yield return new WaitForSeconds(2f);

            if (!cubing) {
                anim.SetTrigger("Attack");
                GameObject projectile = Instantiate(projectilePrefab, new Vector2(projectileSpawnPoint.position.x + direction, projectileSpawnPoint.position.y), projectileSpawnPoint.rotation);

                shooting = false;
            }
        }
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
        timer = 3f;
        if (collision.gameObject.tag == "Player"){
            anim.SetTrigger("Cube");
            cubing = true;
            StopCoroutine(TrocarPosicao());
        }
    }

    void OnTriggerExit2D(Collider2D collision){
        if (collision.gameObject.tag == "Player"){
            anim.SetTrigger("Decube");
            cubing = false;
            shooting = false;
            StartCoroutine(TrocarPosicao());
        }
    }
}