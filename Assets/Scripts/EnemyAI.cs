using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{   
    [SerializeField] public RaycastHit2D rightWall;
    [SerializeField] public RaycastHit2D leftWall;
    [SerializeField] public RaycastHit2D visionEnemy;
    public bool damaged = false;
    private const float TIME_BEFORE_SOUND = 5f;

    
    private enum State {
        Patrolling,
        Idling,
        Attack,
        Jump,
        Stunnado
    }

    private float numero;

    //private int direction = 1;
    private Vector2 direction;

    [Header("Componentes")]
    [SerializeField] public Rigidbody2D enemyRB;
    private Animator anim;

    [Header("Movimento")]
    public float moveSpeed;

    [Header("Raycast")]
    [SerializeField] public Vector2 offSet;
    [SerializeField] public Vector2 offSetVision;
    [SerializeField] public LayerMask layerCollision;
    [SerializeField] public LayerMask layerPlayer;

    [Header("Maquina de Estados")]
    public float idleTimeMin = 1f; 
    public float idleTimeMax = 3f;
    private State currentState;
    private float idleTimer;
    private AudioManager audioPlayer;
    private bool playSound = true; //Só porque eu acho que seria muito irritante ter um rato gritando 
                                   //toda vez que ele mirar em você.
    private float Timer;
    private float Timer2;
    private int randomValue;
    private bool boolState;
    private Vector2 directionVision;
    private float pjDistance;
    private float pjdirection;
    private GameObject jogador;

    [Header("Barra de Stun")]
    [SerializeField] public Transform stunBar;
    [SerializeField] public GameObject stunBarObject;
    private Vida_Inimiga stunScript;
    private float stun;
    private Vector2 stunBarScale;
    private float stunPercent;
    private Vector2 originalScale1;


    // Start is called before the first frame update
    void Start()
    {
        ChoosePatrolDirection();
        audioPlayer = GameObject.FindObjectOfType<SupportScript>().GetComponent<AudioManager>();
        currentState = State.Patrolling;
        idleTimer = Random.Range(idleTimeMin, idleTimeMax);
        jogador = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        stunScript = GetComponent<Vida_Inimiga>();

        originalScale1 = stunBar.localScale;
        stunBarScale = stunBar.localScale;
    }

    void UpdateStunBar(){
        stunPercent = stunScript.currentStun/stunScript.maxStun;
        stunBarScale.x = stunPercent * originalScale1.x;
        stunBar.localScale = stunBarScale;

        if (transform.localScale.x < 0f) {
            stunBarObject.transform.localPosition = new Vector3(0.2f, 0f, 0f);
            stunBarObject.transform.localScale = new Vector3(-1, 1f, 1f);
        }
        else {
            stunBarObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            stunBarObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        // stunBarObject.transform.localScale = new Vector3(transform.localScale.x * transform.localScale.x, 1f, 1f);
        // stunBar.localScale = new Vector3(stunBarScale.x, 1f, 1f);
        // stunBar.localScale = new Vector3(Mathf.Abs(stunBarScale.x), 1f, 1f);
    }
    // Update is called once per frame
    void Update()
    {   
        switch (currentState)
        {
            case State.Patrolling:
                PatrollingState();
                break;
            case State.Idling:
                IdlingState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.Jump:
                JumpState();
                break;
            case State.Stunnado:
                Stunnado_State();
                break;
        }

        if(enemyRB.velocity.x < 0 && currentState != State.Jump)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (enemyRB.velocity.x > 0 && currentState != State.Jump)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        // if (stunScript.currentStun >= stunScript.maxStun){
        if (!stunScript.notStunned) {
            currentState = State.Stunnado;
            Timer = stunScript.stunCooldownTime;
        }

        UpdateStunBar();

        if (gameObject.tag == "enemyIsAttacking" && currentState != State.Jump) {
            gameObject.tag = "Inimigo";
        }

        if (damaged){
            currentState = State.Attack;
            damaged = false;
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void Move()
    {
        if (direction.x > 0f){
            enemyRB.velocity = new Vector2(2f, 0f);
        } else {
            enemyRB.velocity = new Vector2(-2f, 0f);
        }
        
    }

    private void CheckSurroundings(){
        //Direita
        rightWall = Physics2D.Raycast(new Vector2(transform.position.x + offSet.x, transform.position.y + offSet.y), Vector2.right, 1.33f, layerCollision);
        // Debug.DrawRay(new Vector2(transform.position.x + offSet.x, transform.position.y + offSet.y), Vector2.right, Color.yellow);

        leftWall = Physics2D.Raycast(new Vector2(transform.position.x - offSet.x, transform.position.y + offSet.y), Vector2.left, 1.33f, layerCollision);
        // Debug.DrawRay(new Vector2(transform.position.x - offSet.x, transform.position.y + offSet.y), Vector2.left, Color.yellow);

        if (rightWall.collider != null) {
            enemyRB.velocity = new Vector2(-2f, 0f).normalized;
            direction = new Vector2(-1f, 0f).normalized;
            directionVision = direction;
            offSetVision.x = 0.5f;
            Timer = 0.2f;
            idleTimer = Random.Range(idleTimeMin, idleTimeMax);

        }
    
       
        

        if (leftWall.collider != null) {
            enemyRB.velocity = new Vector2(2f, 0f).normalized;
            direction = new Vector2(1f, 0f).normalized;
            directionVision = direction;
            offSetVision.x = -0.5f;
            Timer = 0.2f;
            idleTimer = Random.Range(idleTimeMin, idleTimeMax);
        }
    }

    private void ChoosePatrolDirection()
    {
        // Escolhe uma dire��o de patrulha aleat�ria
        numero = Random.Range(-1f, 1f);
        enemyRB.velocity = new Vector2(2f*numero, 0f).normalized;
        direction = new Vector2(numero, 0f).normalized;
        directionVision = direction;
        offSetVision.x = direction.x/2*-1;
    }

    private void PatrollingState()
    {
        ChangeAnim(false, true, false, false, false);

        Vision();

        Move();
        if (Timer <= 0){
            CheckSurroundings();
        } else {
            Timer -= Time.deltaTime;
        }

        // Muda para o estado de espera ap�s um determinado tempo de patrulha
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            randomValue = Random.Range(0, 2);
            boolState = randomValue != 0;

            if (boolState){
                currentState = State.Idling;
            } else {
                currentState = State.Patrolling;
                ChoosePatrolDirection();
            }
            idleTimer = Random.Range(idleTimeMin, idleTimeMax);
        }
    }

    private void IdlingState()
    {
        ChangeAnim(true, false, false, false, false);

        enemyRB.velocity = new Vector2(0f, 0f);
        Vision();
        // Reduz o temporizador de espera
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {   
            Timer = 0f;
            ChoosePatrolDirection();
            idleTimer = Random.Range(idleTimeMin, idleTimeMax);
            currentState = State.Patrolling;
        }
    }

    private void Vision()
    {   
        Timer2 -= Time.deltaTime;
        visionEnemy = Physics2D.Raycast(new Vector2(transform.position.x - offSetVision.x, transform.position.y + offSetVision.y), directionVision, 4f, layerPlayer);
        Debug.DrawRay(new Vector2(transform.position.x - offSetVision.x, transform.position.y + offSetVision.y), directionVision, Color.blue);

        visionCheck();

        visionEnemy = Physics2D.Raycast(new Vector2(transform.position.x + offSetVision.x, transform.position.y + offSetVision.y), directionVision, 4f, layerPlayer);
        Debug.DrawRay(new Vector2(transform.position.x + offSetVision.x, transform.position.y + offSetVision.y), directionVision, Color.blue);

        visionCheck(); //Só pra eu não ter que colocar o som duas vezes :D
    }

    private void visionCheck(){
        if (visionEnemy.collider != null && Timer2 <= 0)
        {
            if (visionEnemy.collider.CompareTag("Player")){
                currentState = State.Attack;
                if (playSound){
                    audioPlayer.Play("RatAttack");
                    playSound = false;
                    StartCoroutine(TimeSoundReplay(TIME_BEFORE_SOUND));            
                }
            }
        }
    }

    private IEnumerator TimeSoundReplay(float time){
        yield return new WaitForSeconds(time);
        playSound = true;
    }

    private void AttackState(){ 
        pjDistance = Mathf.Sqrt(((jogador.transform.position.x - transform.position.x)*(jogador.transform.position.x - transform.position.x) + 
                      (jogador.transform.position.y - transform.position.y)*(jogador.transform.position.y - transform.position.y)));
        pjdirection = jogador.transform.position.x - transform.position.x;
        if (pjDistance < 3f){
            currentState = State.Jump;
            enemyRB.velocity = new Vector2(0f, 0f);
            Timer = 1.5f;
            Timer2 = 0.5f;
        } else if (pjdirection > 0){
            direction = new Vector2(1f, 0f);
            Move();
        } else {
            direction = new Vector2(-1f, 0f);
            Move();
        }
        
    }

    private void JumpState()
    {   
        pjDistance = Mathf.Sqrt(((jogador.transform.position.x - transform.position.x)*(jogador.transform.position.x - transform.position.x) + 
                      (jogador.transform.position.y - transform.position.y)*(jogador.transform.position.y - transform.position.y)));
{   
        Timer -= Time.deltaTime;

        if (Timer <= 0)
        {
            ChangeAnim(false, false, false, true, false);

            gameObject.tag = "enemyIsAttacking";
            
            if (pjdirection > 0)
            {
                enemyRB.velocity = new Vector2(12f, 4f);

                rightWall = Physics2D.Raycast(new Vector2(transform.position.x + offSet.x, transform.position.y + offSet.y), Vector2.right, 1.7f, layerCollision);
                Debug.DrawRay(new Vector2(transform.position.x + offSet.x, transform.position.y + offSet.y), Vector2.right, Color.yellow);

                if (rightWall.collider != null) {
                    enemyRB.velocity = new Vector2(0f, 0f).normalized;
                }
            }
            else
            {
                enemyRB.velocity = new Vector2(-12f, 4f);

                leftWall = Physics2D.Raycast(new Vector2(transform.position.x - offSet.x, transform.position.y + offSet.y), Vector2.left, 1.7f, layerCollision);
                Debug.DrawRay(new Vector2(transform.position.x - offSet.x, transform.position.y + offSet.y), Vector2.left, Color.yellow);

                if (leftWall.collider != null) {
                enemyRB.velocity = new Vector2(2f, 0f).normalized;
            }
            } 

            Timer2 -= Time.deltaTime;

            if (Timer2 <= 0)
            {
                enemyRB.velocity = new Vector2(0f, 0f);
                Timer2 = 1.5f;
                if (pjDistance > 4f){
                currentState = State.Idling;
                } else {
                    currentState = State.Attack;
                }
            }
        } else
        {
            ChangeAnim(false, false, true, false, false);
            if(pjdirection > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            Timer2 = 0.25f; // Reinicia o Timer2 enquanto o Timer ainda est� ativo
        }

    }

}
    private void ChangeAnim(bool idle, bool patrol, bool prepare, bool jump, bool stun)
    {
        anim.SetBool("Idle", idle);
        anim.SetBool("Patrol", patrol);
        anim.SetBool("Prepare", prepare);
        anim.SetBool("Jump", jump);
        anim.SetBool("Stun", stun);
    }

    private void Stunnado_State(){
        ChangeAnim(false, false, false, false, true);

        enemyRB.velocity = new Vector2(0f, 0f);

        Timer -= Time.deltaTime;

        if (stunScript.notStunned){
            currentState = State.Idling;
        }
        // if (stunScript.currentStun < stunScript.maxStun && Timer <= 0f){
        //     currentState = State.Idling;
        // }
    }
}