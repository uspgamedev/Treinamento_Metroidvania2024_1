using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer; //cria um objeto do tipo groundLayer para ser usado como referencia
    [Space]
    public bool onGround; //se o player esta no chao
    [Space]
    [Header("Collision")]
    public float collisionRadius = 0.25f; //raio da esfera usada para dedectar a colisao
    public Vector2 bottomOffset; //define o quao abaixo do player a origem da esfera estara
    private Color debugCollisionColor = Color.red; //coisa de debug

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {  
        //cria um circulo em baixo do player, se ele colide com algo do tipo groundLayer ele retorna true
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
    }

    //coisa de debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
    }
}
