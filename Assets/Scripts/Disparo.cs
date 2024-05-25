using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Disparo : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D enemyRB;
    public float Timer;
    private bool parry;
    public float projectileSpeed = 5f;
    public float direction = 1f;
    public bool parried = false;
    private bool parryable = true;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake(){
        Timer = 0.1f;
        
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRB = gameObject.GetComponent<Rigidbody2D>();

        if (player.GetComponent<Transform>().position.x > transform.position.x){
            direction= 1f;
        } else {
            direction = -1f;
        }    
            
        
        //s //Poderia só ter apagado, mas quis deixar esse S como recordação. 
    }
    // Update is called once per frame
    void Update()
    {
        Timer -= Time.deltaTime;
        parry = player.GetComponent<PlayerCombat>().isParrying;

        Move();

        if (player.GetComponent<Transform>().localScale.x > 0 && transform.position.x - player.GetComponent<Transform>().position.x > 0) {
            parryable = true;
        } else if(player.GetComponent<Transform>().localScale.x < 0 && transform.position.x - player.GetComponent<Transform>().position.x < 0){
            parryable = true;
        } else {
            parryable = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
       if (collision.gameObject.tag == "Player" && parry && Timer < 0f && parryable){ //Não sei exatametne o que isso faz, só reescrevi de forma a compilar.
           projectileSpeed *= -1f; 
           parried = true;
           Timer = 0.1f;
       } else {
           Destroy(gameObject); 
       }   
    }

    private void Move(){
        enemyRB.velocity = new Vector2(direction*projectileSpeed, 0f);
    }
}

