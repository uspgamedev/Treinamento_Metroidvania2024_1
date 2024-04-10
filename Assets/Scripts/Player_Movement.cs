using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fonte: https://www.youtube.com/watch?v=STyY26a_dPY
public class Player_Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collision coll;
    public float jumpForce = 10;
    public float speed = 50;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>(); //pega o componente Collision do objeto
        rb = GetComponent<Rigidbody2D>(); //pega o componente do objeto
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal"); //pega o input -1, 0 ou 1, a depender se o player esta pressionando a ou d
        float y = Input.GetAxis("Vertical"); //pega o input -1, 0 ou 1, a depender se o player esta pressionando w ou s
        Vector2 dir = new Vector2(x, y); //cria um vetor que representa para quais direcoes o player quer se movimentar
        
        walk(dir);
        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround) //checa se o player esta no chao
                jump(Vector2.up);
        }
    }

    private void walk(Vector2 dir) 
    {
        rb.velocity = (new Vector2(dir.x * speed, rb.velocity.y)); //atualiza o vetor velocidade com o x do vetor dir, note que ele nao muda o y, uma vez que o player so se movimenta
        //verticalmente por meio do pulo
    }

    private void jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }
}
