using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fogo : MonoBehaviour
{
    private BoxCollider2D col;
    private Rigidbody2D rb;

    private ParticleSystem destroyParticles;

    private float originalGravity;
    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator InitializeFogo() {
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(2f);
        rb.gravityScale = originalGravity;
        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Ground") {
            Destroy(gameObject);
        }
    }
}
