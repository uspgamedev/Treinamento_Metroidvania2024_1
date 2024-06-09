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
    [SerializeField] private float TimerDesperta = 3f;
    private float TimerPulando;
    private float TimerEmDash;
    private float nextState;
    private bool dormindo = true;

    private bool canDash = true;
    private bool estaEmDash = false;
    private bool travarScaleDash = false;
    private bool emFogo = false;
    private bool invocando = false;
    private bool idling = false;

    private bool deathIdle = false;

    private bool canPular = true;

    private float vely;

    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private Transform[] limites; 

    public float direction;
    private AudioManager audioPlayer;
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

    private Animator anim;

    [HideInInspector] public bool toDie = false;
    private bool dead;

    float Timer;
    private enum State {
        Controller,
        Idling,
        Dashing, 
        Jumping,
        Firing,
        Invocando,
        Trocando,
        Sleeping,
        DeathIdle
    }

    private State currentState;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ground");

        TimerEmDash = dashDistance/dashSpeed;

        anim = GetComponent<Animator>();
        
        bossRB = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        currentState = State.Sleeping;
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
            positions[j] = new Vector2(child.transform.position.x, 20.5f);
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

        audioPlayer = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>().GetAudioManagerInstance();
    }

    // Update is called once per frame
    void Update()
    {
        if (toDie) {
            currentState = State.DeathIdle;
            dead = true;
            toDie = false;
        }
        switch (currentState){
            case State.Sleeping:
                IdleState();
                break;
            case State.Idling:
                IdleState();
                break;
            case State.DeathIdle:
                DeathIdle();
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

        if (transform.position.x < player.GetComponent<Transform>().position.x && !travarScaleDash){
            direction = 1f;
        } else if (transform.position.x >= player.GetComponent<Transform>().position.x && !travarScaleDash) {
            direction = -1f;
        }

        Timer -= Time.deltaTime;

        if (estaEmDash){ // Quando o Boss est� em dash a velocidade dele � atualizada constantemente
            bossRB.velocity = new Vector2(dashSpeed*direction, 0f);
        }

        if (!dormindo && !travarScaleDash && canPular && !dead) {
            transform.localScale = new Vector3(-direction, 1f, 1f);
        }
    }
    
    private void ChoiceState(){ // Fun��o com as chances de cada novo estado aparecer
        nextState = Random.Range(1, 101);

        if (!dead && !dormindo) {
            if (nextState < 45f){
                currentState = State.Dashing;
            }
            if (nextState >=45f && nextState < 65f ){
                currentState = State.Jumping;
            }
            if (nextState >= 65f && nextState < 85f){
                currentState = State.Firing;
            }
            if (nextState>=85f && nextState <90f){
                currentState = State.Invocando;
            }
            if (nextState>90f){
                currentState = State.Trocando;
            }
        }
    }

    void IdleState(){
        if (!idling) {
            StartCoroutine(Idling());
        }
    }

    void DeathIdle(){
        if (!deathIdle) {
            StartCoroutine(DeathIdling());
        }
    }

    private IEnumerator Idling() {
        idling = true;
        yield return new WaitForSeconds(2f);
        idling = false;
        currentState = State.Controller;
    }
    private IEnumerator DeathIdling() {
        deathIdle = true;
        anim.SetTrigger("StopDash");
        anim.SetTrigger("StopChange");
        anim.SetTrigger("StopLaser");

        canChangeSides = false;

        yield return new WaitForSeconds(0.5f);
        if (!listaA[0].activeInHierarchy) {
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
        }
        audioPlayer.FadeOut("BossBattle_BGM");
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("Die");
    }

    void SleepState(){
        dormindo = true;
        anim.SetBool("Sleeping", true);
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.tag == "Player" && dormindo){ // Quando o Player se aproxima a capivara acorda
            StartCoroutine(Acordando());
        }
    }

    private IEnumerator Acordando(){
        const float TIME_BEFORE_AWAKE = 2.8f;
        dormindo = false; // capivara acordada
        anim.SetTrigger("Wakeup");
        anim.SetBool("Sleeping", false);
        audioPlayer.Play("CapybaraAwake");
        audioPlayer.Play("PreCapybaraFight");
        GetComponent<CircleCollider2D>().enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Enemies");
        
        yield return new WaitForSeconds(TIME_BEFORE_AWAKE);
        audioPlayer.SwitchSound("LadoA_BGM", "BossBattle_BGM");
        yield return new WaitForSeconds(TimerDesperta-TIME_BEFORE_AWAKE); // Enquanto ela desperta deve haver uma anima��o dela levantadando

        currentState = State.Idling; // Depois de acordada, um novo estado � aleatoriamente escolhido (analise a possibilidade de o primeiro estado ser o dash, pra n�o ser t�o ca�tico)
    }

    void DashState(){ 
        if (canDash){ // somente uma flag pra coroutine n�o ser chamada incessantemente
            StartCoroutine(EmDash());
            canDash = false;
        }
    }

    private IEnumerator EmDash(){

        string originalTag = gameObject.tag;

        anim.SetTrigger("Dash");

        yield return new WaitForSeconds(1.5f);

        gameObject.tag = "enemyIsAttacking";
        travarScaleDash = true;

        estaEmDash = true;
        anim.SetTrigger("DashJump");

        yield return new WaitForSeconds(dashDistance/dashSpeed);

        estaEmDash = false;

        Vector2 velocidade = bossRB.velocity; // velocidade inicial para ser usada no DOTween, meio in�til
        DOTween.To(() => velocidade, (x) => bossRB.velocity = x/4, new Vector2(0f, 0f), 2f);
        //S
        yield return new WaitForSeconds(1f);

        gameObject.tag = originalTag;

        yield return new WaitForSeconds(1f);

        anim.SetTrigger("StopDash");
        currentState = State.Idling;
        canDash = true;
        travarScaleDash = false;
    }

    private void JumpState(){

        if (transform.position.x < player.GetComponent<Transform>().position.x){ // Decide para qual lado a capivara pular�, sempre o lado oposto ao player
            dist = limites[0].GetComponent<Transform>().position.x - transform.position.x;
        } else {
            dist = limites[1].GetComponent<Transform>().position.x - transform.position.x;
        }
        if (canPular){ // Flag que inicia a atribui��o das velocidades usando umas f�rmulas
            if (direction > 0f){
            transform.localScale = new Vector3(-1, 1, 1);
            } else {
                transform.localScale = new Vector3(1, 1, 1);
            }
           vely = Mathf.Sqrt(g*Mathf.Abs(dist));
           bossRB.velocity = new Vector2((g/2)*dist/vely, vely);
           TimerPulando = 2*vely/g;
           canPular = false;
           
           StartCoroutine(Pulando()); // Depois de come�ar a pular, a coroutine pulando ir� ativar o raio quando o pulo acabar
        }
    }

    private IEnumerator Pulando(){
        yield return new WaitForSeconds(TimerPulando + 0.5f);
        anim.SetTrigger("Laser");
        yield return new WaitForSeconds(1.5f); // espera um pouco depois que o pulo acaba

        Raio100.GetComponent<Animator>().SetTrigger("Activate");
        Raio100.SetActive(true); // raio
        audioPlayer.Play("CapybaraLaser");

        yield return new WaitForSeconds(TimerRaio);

        Raio100.SetActive(false); // desraio
        anim.SetTrigger("StopLaser");

        currentState = State.Idling;
        canPular = true;
    }

    private void FireState(){
        if (!emFogo){ // Flag intensa
        anim.SetTrigger("Pound");
            primeiro = Random.Range(0, j); // para parecer mais ca�tico, o primeiro espinho da sequ�ncia sempre muda, mas depois continua na ordem normal
            emFogo = true;
            StartCoroutine(AteandoFogo());
        }
    }

    private IEnumerator AteandoFogo(){
        const float TIME_BEFORE_STOMP = 0.3f;
        yield return new WaitForSeconds(TIME_BEFORE_STOMP);
        audioPlayer.Play("CapybaraStomp");
        yield return new WaitForSeconds(1f-TIME_BEFORE_STOMP);
         for (int i=(int)primeiro; i<j;i++){
             chamas[i] = Instantiate(fogoPrefab, positions[i], Quaternion.identity); // usa um array para chamar os objetos
             yield return new WaitForSeconds(0.5f); // intervalinho
         }
         if (primeiro != 0){ // quando chega no final volta pro primeiro, j� que provavelmente o primeiro espinho gerado estr� no meio da sequ�ncia
             for (int i=0; i<primeiro; i++){
                 chamas[i] = Instantiate(fogoPrefab, positions[i], Quaternion.identity);
                 yield return new WaitForSeconds(0.5f);
             }
         }
         emFogo = false;
         currentState = State.Idling;

    }

    private void Invocando(){
        if (!invocando){
            anim.SetTrigger("Change");
            numerodebichos = Random.Range(1, 2); //chama 1 ou 2 bichos
            inimigos = new GameObject[numerodebichos];
            invocando = true;
            StartCoroutine(ChamandoBicho(numerodebichos));
        }
    }

    private IEnumerator ChamandoBicho(int N){
        //anima¿¿¿o antes de invocar
            
            for (int i=0;i<N;i++){
            do {
                enemyposition = Random.Range(limites[0].GetComponent<Transform>().position.x, limites[1].GetComponent<Transform>().position.x);
            } while (enemyposition > transform.position.x - 1f && enemyposition < transform.position.x + 1f); // gaante que o bicho n�o vai surgir em cima do boss
            nextState = Random.Range(-1, 4); // mais chance de aparecer um rato
            yield return new WaitForSeconds(1.5f);
            if (nextState < 0){
                inimigos[i] = Instantiate(blobPrefab, new Vector2(enemyposition, transform.position.y+1f), Quaternion.identity);
            } else {
                inimigos[i] = Instantiate(ratoPrefab, new Vector2(enemyposition, transform.position.y+1f), Quaternion.identity);
            }
            }
            
            yield return new WaitForSeconds(3f);
            anim.SetTrigger("StopChange");
            currentState = State.Idling;
            invocando = false;
    }

    private IEnumerator ChangeSides(){ //troca de lados
        canChangeSides = false;

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(White());
        audioPlayer.Play("ChangeSideSFX");
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
        //anima��o
        canChangeSides = true;
        anim.SetTrigger("StopChange");
        currentState = State.Idling;
    }

    private IEnumerator White(){
        whiteFade.gameObject.SetActive(true);
        whiteFade.DOColor(Color.white, transTime);
        yield return new WaitForSeconds(0f);
    }

    private void Trocando(){
        if(canChangeSides){
            anim.SetTrigger("Change");
            StartCoroutine(ChangeSides());
        }
    }
}
