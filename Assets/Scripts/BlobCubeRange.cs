using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobCubeRange : MonoBehaviour
{
    private Enemy_AI3 blobScript;

    void Awake()
    {
        blobScript = transform.parent.GetComponent<Enemy_AI3>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            blobScript.Cube();
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            blobScript.Decube();
        }
    }
}
