using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Disparo : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D enemyRB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake(){
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRB = gameObject.GetComponent<Rigidbody2D>();
        s
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
       if (isParryingdebug && collision.GameObject.CompareWithTag("Player")){
            enemyRB.velocity = new Vector2(enemyRB.velocity.x*-1f, enemyRB.velocity.y*-1f); 
       } else {
           Destroy(gameObject); 
       }
        
    }
}

