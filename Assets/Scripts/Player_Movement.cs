using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Movement : MonoBehaviour
{
    [HideInInspector] public bool CanMove { get; private set; }
    [HideInInspector] public bool CanMove2 { get; set; } = true;

    private Rigidbody2D rb;
    private Collision coll;
    private Animator anim;

    [Header("Movement")]
    public float jumpForce = 10f;
    public float speed = 50f;

    private bool jumpStart = false;
    private float horizontal;
    private bool canDash = true;
    private bool isDashing = false;
    [HideInInspector] public bool CanGancho { get; set; } = false;
    [HideInInspector] public GameObject VagalumeAtual { get; set; }

    [SerializeField] private float terminalVelocity = 24f;

    [Header("Skills")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    [SerializeField] private float ganchoForce = 1f;

    private SimpleFlash flashScript;
    [HideInInspector] public Vector3 LastPos { get; private set; }
    [HideInInspector] public float LastDir { get; private set; }
    private bool onGroundRec = false;
    private LayerMask groundLayer;
    private float initialGravity;
    private AudioManager audioPlayer;
    private float groundTimeCounter = 0f; // Counter to track how long the player has been on the ground

    private SupportScript support;

    [Header("Particles")]
    [SerializeField] private ParticleSystem landParticle;
    [SerializeField] private ParticleSystem jumpParticle;

    void Awake()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        flashScript = GetComponent<SimpleFlash>();
        groundLayer = LayerMask.NameToLayer("Ground");
        support = FindObjectOfType<SupportScript>();
        initialGravity = rb.gravityScale;
        audioPlayer = support.GetAudioManagerInstance();
    }

    void Start()
    {
        if (support.LastRespawn != Vector3.zero)
        {
            transform.position = support.LastRespawn;
        }
    }

    void Update()
    {
        if (coll.onGround)
        {
            GetComponent<Better_Jumping>().enabled = true;
        }

        if (isDashing)
        {
            return;
        }

        HandleInput();
        UpdateAnimations();
        UpdateDirection();
        ApplyTerminalVelocity();
        CalculateParticles();
        CalculateLastPos();
    }

    private void HandleInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        CanMove = GameObject.FindGameObjectWithTag("BlackFade")?.GetComponent<Image>()?.color.a <= 0.2f;
        if (!CanMove || !CanMove2 && !GetComponent<Health>().onKnockback)
        {
            x = y = 0;
        }

        Vector2 dir = new Vector2(x, y);

        if (GetComponent<PlayerCombat>().isParrying && coll.onGround)
        {
            Walk(Vector2.zero);
        }
        else
        {
            Walk(dir);
        }

        if (Input.GetButtonDown("Jump") && coll.onGround)
        {
            Jump(Vector2.up);
            PlayJump();
        }

        if (Input.GetKeyDown(KeyCode.L) && canDash && support.TemDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.K) && CanGancho && support.TemGancho)
        {
            StartCoroutine(Gancho());
            StartCoroutine(VagalumeAtual.GetComponent<Vagalume>().TakeGancho());
        }
    }

    private void Walk(Vector2 dir)
    {
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
    }

    private void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
        StartCoroutine(JumpAnim());
    }

    private IEnumerator JumpAnim()
    {
        anim.SetBool("OnGround", false);
        jumpStart = true;
        yield return new WaitForSeconds(0.5f);
        jumpStart = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        audioPlayer.Play("Dash");
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = initialGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);

        flashScript.Flash(Color.white);
        canDash = true;
    }

    public IEnumerator Gancho()
    {
        CanGancho = false;
        CanMove2 = false;
        GetComponent<Better_Jumping>().enabled = false;

        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        anim.SetTrigger("Gancho");
        audioPlayer.Play("GanchoGrab");

        yield return new WaitForSeconds(0.25f);

        CanMove2 = true;
        rb.gravityScale = initialGravity;

        Jump(Vector2.up * ganchoForce);
    }

    private void CalculateParticles()
    {
        if (!onGroundRec && coll.onGround)
        {
            audioPlayer.Play("Landing");
            StartCoroutine(PlayLand());
        }
    }

    private IEnumerator PlayLand()
    {
        yield return new WaitForSeconds(0.08f);
        landParticle.Play();
    }

    private void PlayJump()
    {
        audioPlayer.Play("Jump");
        jumpParticle.Play();
    }

    private void UpdateAnimations()
    {
        anim.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("JumpSpeed", Mathf.Abs(rb.velocity.y));
        if (!jumpStart)
        {
            anim.SetBool("OnGround", coll.onGround);
        }
    }

    private void UpdateDirection()
    {
        if (rb.velocity.x < 0 && !GetComponent<Health>().onKnockback)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb.velocity.x > 0 && !GetComponent<Health>().onKnockback)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void ApplyTerminalVelocity()
    {
        if (Mathf.Abs(rb.velocity.y) > terminalVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, -terminalVelocity);
        }
    }

    private void CalculateLastPos() 
    {
        if (coll.onGround)
        {
            groundTimeCounter += Time.deltaTime;

            if (groundTimeCounter >= 0.3f) //Evitando update instantâneo, 
                                           //assim o player não é reespawnado no espinho ou no buraco gerando softlock
            {
                LastPos = transform.position;
                LastDir = transform.localScale.x;

                if (!Physics2D.Raycast(LastPos + new Vector3(LastDir, 0, 0), Vector2.down, 1f, groundLayer))
                {
                    LastPos -= new Vector3(LastDir / 2, 0, 0);
                }
            }
        }
        else
        {
            groundTimeCounter = 0f;
        }

        onGroundRec = coll.onGround;
    }
}
