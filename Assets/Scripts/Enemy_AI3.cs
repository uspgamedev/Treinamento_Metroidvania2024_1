using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Enemy_AI3 : MonoBehaviour
{

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    public float projectileSpeed = 5f;
    public float fireRate = 1f;
    public float direction;
    [SerializeField] private Transform playerTransform;
    private float nextFireTime;


    private enum State {
        Cubing,
        Shootting
    }

    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Shootting;   
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState){
            case State.Shootting:
                ShootingState();
            break;
        }

        if (playerTransform.position.x > transform.position.x){
            direction = 1f;
        } else {
            direction = -1f;
        }

        nextFireTime -= Time.deltaTime;
    }

    void ShootingState()
    {
        if (nextFireTime < 0){
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(projectileSpeed * direction, 0f);
            }
            nextFireTime = 1f/fireRate;
        } 
        
    }
}
