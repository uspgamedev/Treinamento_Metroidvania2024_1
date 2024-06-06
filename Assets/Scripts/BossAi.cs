using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossAi : MonoBehaviour
{   
    private Vector2[] positions;
    
    private Rigidbody2D bossRB;
    private GameObject player;
    private float dist;
    private float g;
    [SerializeField] private float TimerDesperta=3f;
    private float TimerPulando;
    private float TimerEmDash;
    private float nextState;
    private bool dormindo = true;

    private bool canDash = true;
    private bool estaEmDash = false;
    private bool emFogo = false;
    private bool invocando = false;

    private bool canPular = true;

    private float vely;

    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private Transform[] limites; 

    public float direction;
    [SerializeField] private float dashDistance = 2f;
    [SerializeField] private float TimerRaio = 2f;

    [SerializeField] private GameObject Raio100;
    [SerializeField] private GameObject fogoPrefab;

    private int j = 0;

    private float primeiro = 0;
    private float numerodebichos;
    

    private GameObject[] chamas;
    private GameObject[] inimigos;

    [SerializeField] private GameObject blobPrefab;
    [SerializeField] private GameObject ratoPrefab;

    float Timer;
    private enum State {
        Controller,
        Idling,
        Dashing, 
        Jumping,
        Firing
    }

    private State currentState;
    // Start is called before the first frame update
    void Start()
    {
        TimerEmDash = dashDistance/dashSpeed;
        
        bossRB = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        currentState = State.Idling;
        bossRB.velocity = new Vector2(0f, 0f);
        g = 10*bossRB.gravityScale; 
        
        foreach (Transform child in transform){
            if (child.transform != transform){
            j++;
            }
        }
        positions = new Vector2[j];
        j=0;
        foreach(Transform child in transform){
            if (child.transform != transform){
            positions[j] = child.transform.position;
            j++;
            }
        }
        chamas = new GameObject[j];

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState){
            case State.Idling:
                IdleState();
                break;
            case State.Dashing:
                DashState();
                break;
            case State.Jumping:
                JumpState();
                break;
            case State.Firing:
                FireState();
                break;
            case State.Controller:
                ChoiceState();
                break;

        }

        if (transform.position.x < player.GetComponent<Transform>().position.x){
            direction = 1f;
        } else {
            direction = -1f;
        }

        Timer -= Time.deltaTime;
        TimerDesperta -= Time.deltaTime;

        if (estaEmDash){
            bossRB.velocity = new Vector2(dashSpeed*direction, 0f);
        }
    }
    
    private void ChoiceState(){
        nextState = Random.Range(1, 101);

        if (nextState < 50f){
            currentState = State.Dashing;
        }
        if (nextState >=50f && nextState < 70f ){
            currentState = State.Jumping;
        }
        if (nextState >= 70f){
            currentState = State.Firing;
        }
    }

    void IdleState(){
        
    }

    private IEnumerator Acordando(){
        dormindo = false;
        yield return new WaitForSeconds(TimerDesperta);

        currentState = State.Controller;
    }

    private IEnumerator Pulando(){
        yield return new WaitForSeconds(TimerPulando);

        Raio100.SetActive(true);

        yield return new WaitForSeconds(TimerRaio);

        Raio100.SetActive(false);

        currentState = State.Controller;
        canPular = true;
    }

    private IEnumerator EmDash(){
        estaEmDash = true;

        yield return new WaitForSeconds(dashDistance/dashSpeed);

        estaEmDash = false;

        Vector2 velocidade = bossRB.velocity;
        DOTween.To(() => velocidade, (x) => bossRB.velocity = x/4, new Vector2(0f, 0f), 2f);

        yield return new WaitForSeconds(2f);
        currentState = State.Controller;
        canDash = true;
    }


    void DashState(){
        if (canDash){
            StartCoroutine(EmDash());
            canDash = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.tag == "Player" && dormindo){
            StartCoroutine(Acordando());
        }
    }

    private void JumpState(){

        if (transform.position.x < player.GetComponent<Transform>().position.x){
            dist = limites[0].GetComponent<Transform>().position.x - transform.position.x;
        } else {
            dist = limites[1].GetComponent<Transform>().position.x - transform.position.x;
        }
        if (canPular){
            if (direction > 0f){
            transform.localScale = new Vector3(1, 1, 1);
            } else {
                transform.localScale = new Vector3(-1, 1, 1);
            }
           vely = Mathf.Sqrt(g*Mathf.Abs(dist));
           bossRB.velocity = new Vector2((g/2)*dist/vely, vely);
           TimerPulando = 2*vely/g;
           canPular = false;
           
           StartCoroutine(Pulando());
        }
    }

    private void FireState(){
        if (!emFogo){
            Debug.Log("EM FOGO");
            primeiro = Random.Range(0, j);
            emFogo = true;
            StartCoroutine(AteandoFogo());
        }
    }

    private IEnumerator AteandoFogo(){
         for (int i=(int)primeiro; i<j;i++){
             chamas[i] = Instantiate(fogoPrefab, positions[i], Quaternion.identity);
             yield return new WaitForSeconds(0.5f);
         }
         if (primeiro != 0){
             for (int i=0; i<primeiro; i++){
                 chamas[i] = Instantiate(fogoPrefab, positions[i], Quaternion.identity);
                 yield return new WaitForSeconds(0.5f);
             }
         }
         emFogo = false;
         currentState = State.Controller;

    }

    private void Invocando(){
        if (!invocando){
            numerodebichos = Random.Range(3, 6);
            inimigos = new GameObject[(int)numerodebichos];
            StartCoroutine(ChamandoBicho(numerodebichos));

        }
    }

    private IEnumerator ChamandoBicho(float N){
        //animação antes de invocar
        yield return new WaitForSeconds(1.5f);
        float enemyposition = Random.Range(limites[0].GetComponent<Transform>().position.x, limites[1].GetComponent<Transform>().position.x);
        nextState = Random.Range(-1, 1);
        if (nextState < 0){
            inimigos[(int)numerodebichos - (int)N] = Instantiate(blobPrefab, new Vector2(enemyposition, transform.position.y+0.5f), Quaternion.identity);
        } else {
            inimigos[(int)numerodebichos - (int)N] = Instantiate(blobPrefab, new Vector2(enemyposition, transform.position.y+0.5f), Quaternion.identity);
        }
        if (N-1 > 0){
            StartCoroutine(ChamandoBicho(N-1));
        }


    }
}
