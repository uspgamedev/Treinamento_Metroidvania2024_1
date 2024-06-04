using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fogo : MonoBehaviour
{

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake(){

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision){
        Destroy(gameObject);
    }
}
