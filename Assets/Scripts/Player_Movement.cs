using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//fonte: https://www.youtube.com/watch?v=STyY26a_dPY
public class Player_Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool canMove;
    private Collision coll;
    private Animator anim;
    public float jumpForce = 10;
    public float speed = 50;
    private bool jumpStart = false;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>(); //pega o componente Collision do objeto
        rb = GetComponent<Rigidbody2D>(); //pega o componente do objeto
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal"); //pega o input -1, 0 ou 1, a depender se o player esta pressionando a ou d
        float y = Input.GetAxis("Vertical"); //pega o input -1, 0 ou 1, a depender se o player esta pressionando w ou s
        if(GameObject.FindGameObjectWithTag("BlackFade") != null){
            canMove = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>().color.a <= 0.5f;
        //Não sei se entendi direito a lógica do Alpha da unity, mas aparentemente o alpha varia de 0 até 1 (não de 0 a 255)
        //Sendo assim, é só escolher ali quantos porcento da transição deve estar completa pro manito poder andar.
        }

        if (!canMove) x = y = 0;
        Vector2 dir = new Vector2(x, y); //cria um vetor que representa para quais direcoes o player quer se movimentar
        
        walk(dir);
        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround) //checa se o player esta no chao
                jump(Vector2.up);
        }

        anim.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("JumpSpeed", Mathf.Abs(rb.velocity.y));
        if (!jumpStart)
        {
            anim.SetBool("OnGround", coll.onGround);
        }

        if(rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
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
        StartCoroutine("JumpAnim");
    }

    IEnumerator JumpAnim()
    {
        anim.SetBool("OnGround", false);
        jumpStart = true;
        yield return new WaitForSeconds(0.5f);
        jumpStart = false;
    }
}
