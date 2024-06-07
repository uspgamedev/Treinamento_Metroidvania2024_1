using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//fonte: https://www.youtube.com/watch?v=STyY26a_dPY
//fonte: https://gist.github.com/bendux/aa8f588b5123d75f07ca8e69388f40d9
public class Player_Movement : MonoBehaviour
{
    [HideInInspector] public bool canMove;
    [HideInInspector] public bool canMove2 = true;


    private Rigidbody2D rb;
    private Collision coll;
    private Animator anim;

    [Header("Movement")]
    public float jumpForce = 10;
    public float speed = 50;

    private bool jumpStart = false;
    private float horizontal;
    // private bool isFacingRight = true;
    private bool canDash = true;
    private bool isDashing = false;
    [HideInInspector] public bool canGancho = false;
    [HideInInspector] public GameObject vagalumeAtual;

    [SerializeField] private float terminalVelocity = 24f;

    [Header("Skills")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    [SerializeField] private float ganchoForce = 1f;

    private SimpleFlash flashScript;
    [HideInInspector] public Vector3 lastPos;
    [HideInInspector] public float lastDir;
    private bool onGroundRec = false;
    private LayerMask groundLayer;
    private float initialGravity;
    private AudioManager audioPlayer;

    private SupportScript support;

    [Header("Particles")]
    [SerializeField] private ParticleSystem landParticle;
    [SerializeField] private ParticleSystem jumpParticle;

    

    // Start is called before the first frame update
    void Awake()
    {
        coll = GetComponent<Collision>(); //pega o componente Collision do objeto
        rb = GetComponent<Rigidbody2D>(); //pega o componente do objeto
        anim = GetComponent<Animator>();
        flashScript = GetComponent<SimpleFlash>();
        groundLayer = LayerMask.NameToLayer("Ground");
        support = GameObject.FindObjectOfType<SupportScript>().GetComponent<SupportScript>();
        initialGravity = rb.gravityScale;
        audioPlayer = support.getAudioManagerInstance();
        // groundLayer = GameObject.Find("MainTilemapA").layer;

    }

    void Start(){
            Vector3 nulo = new Vector3(0f, 0f, 0f);
            Debug.Log(" first Last Respawn: " + support.lastRespawn);
            if (support.lastRespawn != nulo){
                transform.position = support.lastRespawn;
            }

        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Print());
        if (coll.onGround)
        {
            GetComponent<Better_Jumping>().enabled = true;
        }

        if (isDashing)
        {
            return;
        }

        float x = Input.GetAxis("Horizontal"); //pega o input -1, 0 ou 1, a depender se o player esta pressionando a ou d
        float y = Input.GetAxis("Vertical"); //pega o input -1, 0 ou 1, a depender se o player esta pressionando w ou s
        if(GameObject.FindGameObjectWithTag("BlackFade") != null){
            canMove = GameObject.FindGameObjectWithTag("BlackFade").GetComponent<Image>().color.a <= 0.5f;
        //Não sei se entendi direito a lógica do Alpha da unity, mas aparentemente o alpha varia de 0 até 1 (não de 0 a 255)
        //Sendo assim, é só escolher ali quantos porcento da transição deve estar completa pro manito poder andar.
        }

        if (!canMove || !canMove2 && !GetComponent<Health>().onKnockback) {
            x = 0;
            y = 0;
        }

        Vector2 dir = new Vector2(x, y); //cria um vetor que representa para quais direcoes o player quer se movimentar
        
        if(GetComponent<PlayerCombat>().isParrying == true && coll.onGround) // se isParrying e ele está no chão então ele fica parado
            walk(new Vector2(0, 0));
        else // de resto ele da parry e continua se movimentando normalmente
            walk(dir); 

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround) { //checa se o player esta no chao
                jump(Vector2.up);
                PlayJump();
            }
        }

        anim.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("JumpSpeed", Mathf.Abs(rb.velocity.y));
        if (!jumpStart)
        {
            anim.SetBool("OnGround", coll.onGround);
        }

        if(rb.velocity.x < 0 && !GetComponent<Health>().onKnockback)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb.velocity.x > 0 && !GetComponent<Health>().onKnockback)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.L) && canDash && support.temDash) //tem que mudar aqui se quiser mudar o botão do dash
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.K) && canGancho && support.temGancho)
        {
            StartCoroutine(Gancho());
            StartCoroutine(vagalumeAtual.GetComponent<Vagalume>().TakeGancho());
        }
        
        if (Mathf.Abs(rb.velocity.y) > terminalVelocity) rb.velocity = new Vector2(rb.velocity.x, -terminalVelocity); //implementa a velocidade terminal do player


        CalculateParticles();

        CalculateLastPos();
        
    }

    private IEnumerator Print(){
        yield return new WaitForSeconds(5f);
        Debug.Log("Last Respawn: " + support.lastRespawn);
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
        // GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpForce;
        StartCoroutine("JumpAnim");
    }

    IEnumerator JumpAnim()
    {
        anim.SetBool("OnGround", false);
        jumpStart = true;
        yield return new WaitForSeconds(0.5f);
        jumpStart = false;
    }

    private IEnumerator Dash()
    {
        //esse codigo zera a gravidade e impulsiona o inimigo para o sentido que ele está apontado
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f); 
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = initialGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        
        flashScript.Flash(Color.white);
        canDash = true;
    }

    private void CalculateLastPos()
    {
        if (onGroundRec != coll.onGround) {
            lastPos = transform.position;
            lastDir = transform.localScale.x;
            onGroundRec = coll.onGround;

            if (!Physics2D.Raycast(lastPos + new Vector3(lastDir, 0, 0), Vector2.down, 1f, groundLayer)) {
                lastPos -= new Vector3(lastDir/2, 0, 0);
            }
        }
    }

    public IEnumerator Gancho()
    {
        canGancho = false;
        canMove2 = false;
        GetComponent<Better_Jumping>().enabled = false;

        rb.gravityScale = 0f;
        rb.velocity = new Vector2(0f, 0f);

        anim.SetTrigger("Gancho");
        audioPlayer.Play("GanchoGrab");

        yield return new WaitForSeconds(0.25f);

        canMove2 = true;
        rb.gravityScale = initialGravity;

        jump(Vector2.up * ganchoForce);
    }

    private void CalculateParticles() {
        if (!onGroundRec && coll.onGround) {
            audioPlayer.Play("Landing");        
            StartCoroutine(PlayLand());
        }
    }

    private IEnumerator PlayLand() {
        yield return new WaitForSeconds(0.08f);
        landParticle.Play();
    }

    private void PlayJump() {
        audioPlayer.Play("Jump");
        jumpParticle.Play();
    }
}
