using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentativaCamera : MonoBehaviour
{
    [SerializeField] private GameObject lacamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D (Collider2D coll)
    {
        lacamera = transform.GetChild(0).gameObject;
        lacamera.SetActive(!lacamera.activeInHierarchy);
    }

    void OnTriggerExit2D (Collider2D coll)
    {
        lacamera = transform.GetChild(0).gameObject;
        lacamera.SetActive(!lacamera.activeInHierarchy);
    }
}
