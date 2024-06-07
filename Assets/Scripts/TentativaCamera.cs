using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TentativaCamera : MonoBehaviour
{
    private GameObject virtualCam;
    private CinemachineVirtualCamera vcam;
    private Transform player;

    [SerializeField] private bool isRespawn;
    [SerializeField] private Transform respawnPoint;


    private SupportScript support;
    
    void Awake()
    {
        virtualCam = transform.GetChild(0).gameObject;
        player = GameObject.Find("Player").transform;
        vcam = virtualCam.GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = player;
        
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();

        

    }

    void OnTriggerEnter2D (Collider2D coll)
    {
        if (coll.tag == "Player" && !virtualCam.activeInHierarchy) {
            virtualCam.SetActive(true);
            if (isRespawn) {
                PlayerPrefs.SetFloat("pos_x", respawnPoint.position.x);
                PlayerPrefs.SetFloat("pos_y", respawnPoint.position.y);
                PlayerPrefs.SetFloat("pos_z", respawnPoint.position.z);
            }
        }
    }

    void OnTriggerExit2D (Collider2D coll)
    {
        if (coll.tag == "Player") {
            virtualCam.SetActive(false);
        }
    }
}
