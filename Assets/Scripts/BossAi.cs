using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BossAi : MonoBehaviour
{   
    private GameObject[] listaA;
    private GameObject[] listaB;
    
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
    private int numerodebichos;
    private float enemyposition;
    

    private GameObject[] chamas;
    private GameObject[] inimigos;

    [SerializeField] private GameObject blobPrefab;
    [SerializeField] private GameObject ratoPrefab;

    [HideInInspector] public bool canChangeSides;

    private SupportScript support;
    [SerializeField] private float transTime; 
    private Image whiteFade;

    float Timer;
    private enum State {
        Controller,
        Idling,
        Dashing, 
        Jumping,
        Firing,
        Invocando,
        Trocando
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

        listaA = GameObject.FindObjectOfType<SupportScript>().GetComponent<SupportScript>().listaA;
        listaB = GameObject.FindObjectOfType<SupportScript>().GetComponent<SupportScript>().listaB;

        GameObject[] listaTemp = FindObjectsOfType<GameObject>(true);

        canChangeSides = true;

        whiteFade = GameObject.Find("WhiteFade").GetComponent<Image>();
        whiteFade.color = new Color(1f, 1f, 1f, 0f);


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
            case State.Invocando:
                Invocando();
                break;
            case State.Controller:
                ChoiceState();
                break;
            case State.Trocando:
                Trocando();
                break;

        }

        if (transform.position.x < player.GetComponent<Transform>().position.x){
            direction = 1f;
        } else {
            direction = -1f;
        }

        Timer -= Time.deltaTime;

        if (estaEmDash){ // Quando o Boss está em dash a velocidade dele é atualizada constantemente
            bossRB.velocity = new Vector2(dashSpeed*direction, 0f);
        }
    }
    
    private void ChoiceState(){ // Função com as chances de cada novo estado aparecer
        nextState = Random.Range(1, 101);

        if (nextState < 55f){
            currentState = State.Dashing;
        }
        if (nextState >=55f && nextState < 75f ){
            currentState = State.Jumping;
        }
        if (nextState >= 75f && nextState < 85f){
            currentState = State.Firing;
        }
        if (nextState>=85f && nextState <90f){
            currentState = State.Invocando;
        }
        if (nextState>90f){
            currentState = State.Trocando;
        }
    }

    void IdleState(){
        //Sprite da Capivara Dormindo
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.tag == "Player" && dormindo){ // Quando o Player se aproxima a capivara acorda
            StartCoroutine(Acordando());
        }
    }

    private IEnumerator Acordando(){
        dormindo = false; // capivara acordada
        yield return new WaitForSeconds(TimerDesperta); // Enquanto ela desperta deve haver uma animação dela levantadando

        currentState = State.Controller; // Depois de acordada, um novo estado é aleatoriamente escolhido (analise a possibilidade de o primeiro estado ser o dash, pra não ser tão caótico)
    }

    void DashState(){ 
        if (canDash){ // somente uma flag pra coroutine não ser chamada incessantemente
            StartCoroutine(EmDash());
            canDash = false;
        }
    }

    private IEnumerator EmDash(){
        estaEmDash = true;

        yield return new WaitForSeconds(dashDistance/dashSpeed);

        estaEmDash = false;

        Vector2 velocidade = bossRB.velocity; // velocidade inicial para ser usada no DOTween, meio inútil
        DOTween.To(() => velocidade, (x) => bossRB.velocity = x/4, new Vector2(0f, 0f), 2f);
        //S
        yield return new WaitForSeconds(2f);
        currentState = State.Controller;
        canDash = true;
    }

    private void JumpState(){

        if (transform.position.x < player.GetComponent<Transform>().position.x){ // Decide para qual lado a capivara pulará, sempre o lado oposto ao player
            dist = limites[0].GetComponent<Transform>().position.x - transform.position.x;
        } else {
            dist = limites[1].GetComponent<Transform>().position.x - transform.position.x;
        }
        if (canPular){ // Flag que inicia a atribuição das velocidades usando umas fórmulas
            if (direction > 0f){
            transform.localScale = new Vector3(1, 1, 1);
            } else {
                transform.localScale = new Vector3(-1, 1, 1);
            }
           vely = Mathf.Sqrt(g*Mathf.Abs(dist));
           bossRB.velocity = new Vector2((g/2)*dist/vely, vely);
           TimerPulando = 2*vely/g;
           canPular = false;
           
           StartCoroutine(Pulando()); // Depois de começar a pular, a coroutine pulando irá ativar o raio quando o pulo acabar
        }
    }

    private IEnumerator Pulando(){
        yield return new WaitForSeconds(TimerPulando+3f); // espera um pouco depois que o pulo acaba

        Raio100.SetActive(true); // raio

        yield return new WaitForSeconds(TimerRaio);

        Raio100.SetActive(false); // desraio

        currentState = State.Controller;
        canPular = true;
    }

    private void FireState(){
        if (!emFogo){ // Flag intensa
            primeiro = Random.Range(0, j); // para parecer mais caótico, o primeiro espinho da sequência sempre muda, mas depois continua na ordem normal
            emFogo = true;
            StartCoroutine(AteandoFogo());
        }
    }

    private IEnumerator AteandoFogo(){
         for (int i=(int)primeiro; i<j;i++){
             chamas[i] = Instantiate(fogoPrefab, positions[i], Quaternion.identity); // usa um array para chamar os objetos
             yield return new WaitForSeconds(0.5f); // intervalinho
         }
         if (primeiro != 0){ // quando chega no final volta pro primeiro, já que provavelmente o primeiro espinho gerado estrá no meio da sequência
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
            numerodebichos = Random.Range(1, 2); //chama 1 ou 2 bichos
            inimigos = new GameObject[numerodebichos];
            invocando = true;
            StartCoroutine(ChamandoBicho(numerodebichos));

        }
    }

    private IEnumerator ChamandoBicho(int N){
        //animação antes de invocar
        
            
            for (int i=0;i<N;i++){
            do {
                enemyposition = Random.Range(limites[0].GetComponent<Transform>().position.x, limites[1].GetComponent<Transform>().position.x);
            } while (enemyposition > transform.position.x - 1f && enemyposition < transform.position.x + 1f); // gaante que o bicho não vai surgir em cima do boss
            nextState = Random.Range(-1, 4); // mais chance de aparecer um rato
            yield return new WaitForSeconds(1.5f);
            if (nextState < 0){
                inimigos[i] = Instantiate(blobPrefab, new Vector2(enemyposition, transform.position.y+1f), Quaternion.identity);
            } else {
                inimigos[i] = Instantiate(ratoPrefab, new Vector2(enemyposition, transform.position.y+1f), Quaternion.identity);
            }
            }
            
            invocando = false;
            yield return new WaitForSeconds(3f);
            currentState = State.Controller;
    }

    private IEnumerator ChangeSides(){ //troca de lados
        Debug.Log("Começou");
        canChangeSides = false;


        StartCoroutine(White());
        yield return new WaitForSeconds(transTime);

        foreach (GameObject objeto in listaA)
        {
            if (objeto != null)
            objeto.SetActive(!objeto.activeInHierarchy);
        }
        foreach (GameObject objeto in listaB)
        {
            if (objeto != null)
                objeto.SetActive(!objeto.activeInHierarchy);
        }

        yield return new WaitForSeconds(transTime/2);
        //animação
        yield return new WaitForSeconds(transTime/2);

        canChangeSides = true;
        currentState = State.Controller;
        
    }

    private IEnumerator White(){
        whiteFade.DOColor(Color.white, transTime);
        yield return new WaitForSeconds(0f);
    }

    private void Trocando(){
        if(canChangeSides){
            Debug.Log("Quase lá");
            StartCoroutine(ChangeSides());
        }
    }
}
