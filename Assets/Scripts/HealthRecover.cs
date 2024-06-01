using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthRecover : MonoBehaviour
{
    [SerializeField] private int hpRecovered;
    private float despawnTime;
    
    private Rigidbody2D rb;
    private CircleCollider2D col;

    [HideInInspector] public bool goRight;

    void Awake()
    {
        despawnTime = GetComponent<SimpleFlash>().duration;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();

        StartCoroutine(ColliderInit());


        if (goRight) {
            rb.velocity = new Vector2(Random.Range(0.5f, 2.5f), Random.Range(0.5f, 2f));
        }
        else {
            rb.velocity = new Vector2(Random.Range(-2.5f, -0.5f), Random.Range(0.5f, 2f));
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;

        col.isTrigger = true;
        col.radius *= 1.25f;

        if (other.gameObject.tag == "Player") {
            Collect(other.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            Collect(other.gameObject);
        }
    }

    private IEnumerator Despawn() {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }

    private void Collect(GameObject player)
    {
        player.GetComponent<Health>().HealthRestore(hpRecovered);
        GetComponent<SimpleFlash>().Flash(Color.green);
        StartCoroutine(Despawn());
    }

    private IEnumerator ColliderInit() {
        col.enabled = false;
        yield return new WaitForSeconds(0.15f);
        col.enabled = true;
    }
}
