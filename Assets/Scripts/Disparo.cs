using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Disparo : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D enemyRB;
    private Animator anim;
    private new Light2D light;

    public float Timer;
    private bool parry;
    public float projectileSpeed = 5f;
    public float direction = 1f;
    public bool parried = false;
    private bool parryable = true;
    private float blinkTime = 0.5f;

    private Vector2 versor;


    void Awake(){
        Timer = 0.2f;
        
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRB = gameObject.GetComponent<Rigidbody2D>();
        light = transform.GetChild(0).GetComponent<Light2D>();

        versor = new Vector2(player.GetComponent<Transform>().position.x - transform.position.x, player.GetComponent<Transform>().position.y - transform.position.y);
        versor = versor.normalized;

        /*if (player.GetComponent<Transform>().position.x > transform.position.x){
            direction= 1f;
        } else {
            direction = -1f;
        }   */ 
            
        
        //s //Poderia só ter apagado, mas quis deixar esse S como recordação. 
    }

    void Update()
    {
        //Timer -= Time.deltaTime;
        parry = player.GetComponent<PlayerCombat>().isParrying;

        Move();

        if (player.GetComponent<Transform>().localScale.x > 0 && transform.position.x - player.GetComponent<Transform>().position.x > 0) {
            parryable = true;
        } else if(player.GetComponent<Transform>().localScale.x < 0 && transform.position.x - player.GetComponent<Transform>().position.x < 0){
            parryable = true;
        } else {
            parryable = false;
        }

        if (blinkTime > 0.5f) {
            blinkTime = 0f;
            Blink();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       if (collision.gameObject.tag == "Player" && parry && parryable){ //Não sei exatametne o que isso faz, só reescrevi de forma a compilar.
           projectileSpeed *= -1f; 
           parried = true;
       } else {
           if (collision.gameObject.tag == "Blob" && parried){
                Destroy(collision.gameObject); 
           }
           Destroy(gameObject);
       }  
    }

    private void Move(){
        enemyRB.velocity = new Vector2(versor.x*projectileSpeed, versor.y*projectileSpeed);
    }

    private void Blink() {
        if (light.intensity > 1.25f) {
            light.intensity = 1f;
        }
        if (light.intensity < 1.25f) {
            light.intensity = 1.5f;
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.tag=="Blob" && parried){
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }*/
}