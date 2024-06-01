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

    private bool parry;
    [HideInInspector] public float projectileSpeed;
    [HideInInspector] public float direction = 1f;
    [HideInInspector] public bool parried = false;
    private bool parryable = true;
    private float blinkTime = 0.5f;

    private Vector2 versor;

    [SerializeField] private ParticleSystem trailParticles;
    [SerializeField] private ParticleSystem dieParticles;
    private ParticleSystem.VelocityOverLifetimeModule partVel;
    private ParticleSystem.VelocityOverLifetimeModule dieVel;

    void Awake(){
        
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRB = gameObject.GetComponent<Rigidbody2D>();
        light = transform.GetChild(0).GetComponent<Light2D>();

        versor = new Vector2(player.GetComponent<Transform>().position.x - transform.position.x, player.GetComponent<Transform>().position.y - transform.position.y);
        versor = versor.normalized;

        partVel = trailParticles.velocityOverLifetime;
        dieVel = dieParticles.velocityOverLifetime;

        //s //Poderia só ter apagado, mas quis deixar esse S como recordação. 
    }

    void Update()
    {
        parry = player.GetComponent<PlayerCombat>().isParrying;

        Move();

        if (player.GetComponent<Transform>().localScale.x > 0 && transform.position.x > player.GetComponent<Transform>().position.x) {
            parryable = true;
        } else if(player.GetComponent<Transform>().localScale.x < 0 && transform.position.x < player.GetComponent<Transform>().position.x){
            parryable = true;
        } else {
            parryable = false;
        }

        if (blinkTime > 0.5f) {
            blinkTime = 0f;
            Blink();
        }

        partVel.x = new ParticleSystem.MinMaxCurve(-2 * versor.x);
        partVel.y = new ParticleSystem.MinMaxCurve(-2 * versor.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       if (collision.gameObject.tag == "Player" && parry && parryable) {
           versor *= -2.5f; 
           parried = true;
       } else {
           if (collision.gameObject.tag == "Blob" && parried){
                Enemy_AI3 enemyScript = collision.gameObject.GetComponent<Enemy_AI3>();
                enemyScript.StartNewCoroutine(enemyScript.Die());
           }
           StartCoroutine(Die());
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

    private IEnumerator Die() {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        light.gameObject.SetActive(false);
        trailParticles.gameObject.SetActive(false);
        
        if (versor.x > 0f) {
            dieVel.x = new ParticleSystem.MinMaxCurve(-0.4f);
        }
        else {
            dieVel.x = new ParticleSystem.MinMaxCurve(0.4f);
        }

        dieParticles.Play();

        yield return new WaitForSeconds(dieParticles.main.startLifetime.constant);

        Destroy(gameObject);
    }
}