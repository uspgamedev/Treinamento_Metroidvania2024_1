using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{   
    public RaycastHit2D rightWall;
    public RaycastHit2D leftWall;
    public RaycastHit2D visionEnemy;

    
    private enum State {
        Patrolling,
        Idling,
        Attack,
        Jump
    }

    private float numero;

    //private int direction = 1;
    private Vector2 direction;
    [Header("Componentes")]
    public Rigidbody2D enemyRB;

    [Header("Movimento")]
    public float moveSpeed;

    [Header("Raycast")]
    public Vector2 offSet;
    public Vector2 offSetVision;
    public LayerMask lala;
    public LayerMask layerVision;

    [Header("Máquina_de_Estados")]
    public float idleTimeMin = 1f; 
    public float idleTimeMax = 3f;
    private State currentState;
    private float idleTimer;
    private float Timer;
    private float Timer2;
    private int randomValue;
    private bool boolState;
    private Vector2 directionVision;
    private float pjDistance;
    private float pjdirection;
    private GameObject jogador;

    // Start is called before the first frame update
    void Start()
    {
        ChoosePatrolDirection();
        currentState = State.Patrolling;
        idleTimer = Random.Range(idleTimeMin, idleTimeMax);
        jogador = GameObject.Find("Player");
        
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
        rightWall = Physics2D.Raycast(new Vector2(transform.position.x + offSet.x, transform.position.y + offSet.y), Vector2.right, 1f, lala);
        Debug.DrawRay(new Vector2(transform.position.x + offSet.x, transform.position.y + offSet.y), Vector2.right, Color.yellow);

        leftWall = Physics2D.Raycast(new Vector2(transform.position.x - offSet.x, transform.position.y + offSet.y), Vector2.left, 1f, lala);
        Debug.DrawRay(new Vector2(transform.position.x - offSet.x, transform.position.y + offSet.y), Vector2.left, Color.yellow);

        if (rightWall.collider != null) {
            enemyRB.velocity = new Vector2(-2f, 0f).normalized;
            Debug.Log("ola");
            direction = new Vector2(-1f, 0f).normalized;
            directionVision = direction;
            offSetVision.x = 0.5f;
            Timer = 0.5f;
        }
    
       
        

        if (leftWall.collider != null) {
            enemyRB.velocity = new Vector2(2f, 0f).normalized;
            Debug.Log("ola");
            direction = new Vector2(1f, 0f).normalized;
            directionVision = direction;
            offSetVision.x = -0.5f;
            Timer = 0.5f;
        }
    }

    private void ChoosePatrolDirection()
    {
        // Escolhe uma direção de patrulha aleatória
        numero = Random.Range(-1f, 1f);
        enemyRB.velocity = new Vector2(2f*numero, 0f).normalized;
        direction = new Vector2(numero, 0f).normalized;
        directionVision = direction;
        offSetVision.x = direction.x/2*-1;
    }

    private void PatrollingState()
    {   
        Vision();
        
        Move();
        if (Timer <= 0){
            CheckSurroundings();
        } else {
            Timer -= Time.deltaTime;
        }

        // Muda para o estado de espera após um determinado tempo de patrulha
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            randomValue = Random.Range(0, 2);
            boolState = (randomValue == 0) ? false : true;

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
        enemyRB.velocity = new Vector2(0f, 0f);
        Vision();
        // Reduz o temporizador de espera
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {   
            Timer = 1f;
            ChoosePatrolDirection();
            idleTimer = Random.Range(idleTimeMin, idleTimeMax);
            currentState = State.Patrolling;
        }
    }

    private void Vision()
    {   
        Timer2 -= Time.deltaTime;
        visionEnemy = Physics2D.Raycast(new Vector2(transform.position.x - offSetVision.x, transform.position.y + offSetVision.y), directionVision, 2.5f, layerVision);
        Debug.DrawRay(new Vector2(transform.position.x - offSetVision.x, transform.position.y + offSetVision.y), directionVision, Color.blue);

        if (visionEnemy.collider != null && Timer2 <= 0)
        {
            if (visionEnemy.collider.CompareTag("Player")){
                pjDistance = jogador.transform.position.x - transform.position.x;
                currentState = State.Attack;
            }
        }
    }

    private void AttackState(){
        Debug.Log("atacando");
        Vision();
        pjDistance = Mathf.Sqrt(((jogador.transform.position.x - transform.position.x)*(jogador.transform.position.x - transform.position.x) + 
                      (jogador.transform.position.y - transform.position.y)*(jogador.transform.position.y - transform.position.y)));
        pjdirection = jogador.transform.position.x - transform.position.x;
        if (pjDistance < 2f){
            currentState = State.Jump;
            enemyRB.velocity = new Vector2(0f, 0f);
            Debug.Log("uai");
            Timer = 1f;
            Timer2 = 0.5f;
        } else if (pjdirection > 0){
            direction = new Vector2(1f, 0f);
            Move();
        } else {
            direction = new Vector2(-1f, 0f);
            Move();
        }
        
        //currentState = State.Patrolling;
    }

    private void JumpState()
    {   
{   
        Timer -= Time.deltaTime;

        if (Timer <= 0)
        {
            if (pjdirection > 0)
            {
                enemyRB.velocity = new Vector2(8f, 6f);
            }
            else
            {
                enemyRB.velocity = new Vector2(-8f, 6f);
            }

            Timer2 -= Time.deltaTime;

            if (Timer2 <= 0)
            {
                enemyRB.velocity = new Vector2(0f, 0f);
                Timer2 = 1.5f;
                currentState = State.Idling;
            }
        } else
        {
            Timer2 = 0.25f; // Reinicia o Timer2 enquanto o Timer ainda está ativo
        }

    }
}
}
