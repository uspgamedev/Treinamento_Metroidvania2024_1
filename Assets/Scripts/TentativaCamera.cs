using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TentativaCamera : MonoBehaviour
{
    private GameObject virtualCam;


    void Start()
    {
        virtualCam = transform.GetChild(0).gameObject;
    }

    void OnTriggerEnter2D (Collider2D coll)
    {
        if (coll.tag == "Player" && !virtualCam.activeInHierarchy) {
            virtualCam.SetActive(true);
        }
    }

    void OnTriggerExit2D (Collider2D coll)
    {
        if (coll.tag == "Player") {
            virtualCam.SetActive(false);
        }
    }
}
