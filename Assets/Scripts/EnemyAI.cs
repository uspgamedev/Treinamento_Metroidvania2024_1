using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{   
    public RaycastHit2D rightWall;
    public RaycastHit2D leftWall;
    private RaycastHit2D rightLedge;
    private RaycastHit2D leftLedge;

    
    private enum State {
        Patrolling,
        Idling
    }

    //private int direction = 1;
    private Vector2 direction;
    [Header("Componentes")]
    public Rigidbody2D enemyRB;

    [Header("Movimento")]
    public float moveSpeed;

    [Header("Raycast")]
    public Vector2 offSet;
    public LayerMask lala;

    [Header("Máquina_de_Estados")]
    public float idleTimeMin = 1f; 
    public float idleTimeMax = 3f;
    private State currentState;
    private float idleTimer;
    private int randomValue;
    private bool boolState;

    // Start is called before the first frame update
    void Start()
    {
        ChoosePatrolDirection();
        currentState = State.Patrolling;
        idleTimer = Random.Range(idleTimeMin, idleTimeMax);
        
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
        }
    }

    private void FixedUpdate()
    {

    }

    private void Move()
    {
        //enemyRB.velocity = new Vector2(moveSpeed*direction, enemyRB.velocity.y);
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void CheckSurroundings(){
        //Direita
        rightWall = Physics2D.Raycast(new Vector2(transform.position.x + offSet.x, transform.position.y + offSet.y), Vector2.right, 1f, lala);
        Debug.DrawRay(new Vector2(transform.position.x + offSet.x, transform.position.y + offSet.y), Vector2.right, Color.yellow);

        if (rightWall.collider != null) {
            direction *= -1f;
            Debug.Log("ola");
        }
    
        //Esquerda
        leftWall = Physics2D.Raycast(new Vector2(transform.position.x - offSet.x, transform.position.y + offSet.y), Vector2.left, 1f, lala);
        Debug.DrawRay(new Vector2(transform.position.x - offSet.x, transform.position.y + offSet.y), Vector2.left, Color.yellow);

        if (leftWall.collider != null) {
            direction *= -1f;
            Debug.Log("ola");
        }
    }

    private void ChoosePatrolDirection()
    {
        // Escolhe uma direção de patrulha aleatória
        direction = new Vector3(Random.Range(-1f, 1f), 0f, 0f).normalized;
    }

    private void PatrollingState()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        CheckSurroundings();

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
        // Reduz o temporizador de espera
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
}
