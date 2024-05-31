using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAi : MonoBehaviour
{   
    private Rigidbody2D bossRB;
    private GameObject player;
    private float dist;

    private float vely;

    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private Transform[] limites; 

    private float direction;



    float Timer;
    private enum State {
        Idling,
        Dashing, 
        Jumping
    }

    private State currentState;
    // Start is called before the first frame update
    void Start()
    {
        bossRB = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        currentState = State.Jumping;
        bossRB.velocity = new Vector2(0f, 0f);

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
        }

        if (transform.position.x < player.GetComponent<Transform>().position.x){
            direction = 1f;
        } else {
            direction = -1f;
        }

        Timer -= Time.deltaTime;
    }
    
    void IdleState(){
     
    }

    void DashState(){
        

        if (Timer>0f){
            bossRB.velocity = new Vector2(direction*dashSpeed, 0f);
        } else {
            bossRB.velocity = new Vector2(0f, 0f);
            currentState = State.Idling;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.tag == "Player"){
            currentState = State.Dashing;
            Timer = 0.5f;
        }
    }

    private void JumpState(){

        if (transform.position.x < player.GetComponent<Transform>().position.x){
            dist = limites[0].GetComponent<Transform>().position.x - transform.position.x;
        } else {
            dist = limites[1].GetComponent<Transform>().position.x - transform.position.x;
        }
       
       
       
       //
      
       vely = Mathf.Sqrt(23*Mathf.Abs(dist));
       bossRB.velocity = new Vector2(20*dist/vely, vely);
       currentState = State.Idling;
    }
}
