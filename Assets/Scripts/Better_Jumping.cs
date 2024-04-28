using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Better_Jumping : MonoBehaviour
{
    public float fallMultiplier = 2.5f; //define a diferenca entre a velocidade de subida e a velocidade de descida no pulo,
    //quanto maior, mais rapida sera a descida do player em comparacao com a sua subida
    public float lowJumpMultiplier = 2f; //define a diferença na altura do pulo entre pressionar o botao de pulo rapidamente e segurar o botao de pulo,
    //quanto maior lowJumpMultiplier, maior sera a diferenca entre um pulo rapido e um pulo demorado
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (rb.velocity.y < 0) //quando o player comeca a descer no pulo, o jogo acentua a gravidade de acordo com fallMultiplier
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) //quando o player comeca a subir no pulo, o jogo aumenta a gravidade (pulo menor) se o jogador nao estiver segurando o botao de pulo
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
