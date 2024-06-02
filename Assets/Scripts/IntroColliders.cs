using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroColliders : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            Destroy(transform.parent.gameObject);
        }
    }
}
