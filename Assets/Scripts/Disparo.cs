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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake(){
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRB = gameObject.GetComponent<Rigidbody2D>();
        //s //Poderia só ter apagado, mas quis deixar esse S como recordação. 
    }
    // Update is called once per frame
    void Update()
    {
        Timer -= Time.deltaTime;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
       if (collision.gameObject.tag == "Player" && player.GetComponent<PlayerCombat>().isParrying && Timer < 0f){ //Não sei exatametne o que isso faz, só reescrevi de forma a compilar.
           
            enemyRB.velocity = new Vector2(-1f*enemyRB.velocity.x, 0f);
       } else {
           Destroy(gameObject); 
       }
        
    }
}

