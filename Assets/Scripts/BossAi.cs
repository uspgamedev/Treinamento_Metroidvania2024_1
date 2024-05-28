using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAi : MonoBehaviour
{   
    Rigidbody2D bossRB;

    float direction;

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
        }

        Timer -= Time.deltaTime;
    }

    void IdleState(){

    }

    void DashState(){
        Timer = 0.5f;

        if (Timer>0f){
            bossRB.velocity = new Vector2();
        }
    }
}
