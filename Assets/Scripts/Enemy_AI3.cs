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
    private float fireRate = 1f;

    private float Timer =0f;

    [SerializeField] private Transform[] positions;
    private int k=0;


    private enum State {
        Cubing,
        Shootting
    }

    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Shootting;   
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        StartCoroutine(TrocarPosicao());
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState){
            case State.Shootting:
                ShootingState();
                break;
            case State.Cubing:
                CubingState();
                break;
        }

        if (playerTransform.position.x > transform.position.x){
            direction = 1f;
        } else {
            direction = -1f;
        }

        nextFireTime -= Time.deltaTime;
        Timer -= Time.deltaTime;

        
    }

    private IEnumerator TrocarPosicao() {
        yield return new WaitForSeconds(Random.Range(10, 15));

        if (k<positions.Length-1){
            k = k+1;
        } else {
            k=0;
        }

        transform.position = positions[k].position;

        if (currentState != State.Cubing){
            StartCoroutine(TrocarPosicao());
        }
        
    }

    void ShootingState()
    {
        if (nextFireTime < 0){
            GameObject projectile = Instantiate(projectilePrefab, new Vector2(projectileSpawnPoint.position.x + direction, projectileSpawnPoint.position.y), projectileSpawnPoint.rotation);
            
            nextFireTime = 1f/fireRate;
        } 
        
    }

    void CubingState()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Timer = 3f;
        if (collision.gameObject.tag == "Player"){
            currentState = State.Cubing;
        }
    }

    void OnTriggerExit2D(Collider2D collision){
        if (collision.gameObject.tag == "Player"){
            currentState = State.Shootting;
        }

        if (Timer <=0f){
        StartCoroutine(TrocarPosicao());
        }
    }
    
}
