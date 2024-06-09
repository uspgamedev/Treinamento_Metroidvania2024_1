using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.PlayerLoop;

//Fonte: Otávio

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask enemiesLayer; //qual a layer dos inimigos;
    [SerializeField] private Transform attackPoint; //define um ponto (um objeto) de referência para calcular o alcance do ataque;
    [SerializeField] private ParticleSystem parryParticles;
    private Animator anim;

    [Header("Stats")]
    [SerializeField] private float attackRange; //range de ataque (perceba que ele é calculado a partir do attackPoint);
    [SerializeField] private int attackDamage; //dano de stun do ataque;
    [SerializeField] private int parryDamage;

    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isParrying = false;
    private bool followUp = false;
    private AudioManager audioPlayer;
    private SupportScript support;
    private Health healthScript;
    void Start()
    {
        anim = GetComponent<Animator>();
        support = GameObject.FindObjectOfType<SupportScript>().GetComponent<SupportScript>();
        healthScript = GetComponent<Health>();
        audioPlayer = support.GetAudioManagerInstance();
    }


    void Update()
    {
        //se J foi pressionado ataque;
        if (Input.GetKeyDown(KeyCode.J)) //tem que mudar esse botao para mudar o botao do ataque
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.K) && !GetComponent<Player_Movement>().CanGancho && support.temParry) //tem que mudar esse botao para mudar o botao do parry
        {
            Parry();
        }
    }

    void Attack()
    {
        if (!isAttacking && !isParrying)
        {
            StartCoroutine(OnAttack()); //chama função que torna isAttacking = true até que o limite de tempo entre ataques passe;

            anim.SetBool("FollowUp", followUp);
            anim.SetTrigger("Attack");

            if (!followUp) {
                StartCoroutine("AttackFollowUp");
            }
            else {
                StopCoroutine("AttackFollowUp");
                followUp = false;
            }

            //retorna uma lista com todos os inimigos que estão dentro do range de ataque;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, enemiesLayer);
            bool collidedWithSomeone = false;
            //para cada inimigo no range de ataque chame TakeDamage;
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.gameObject.GetComponent<Vida_Inimiga>() != null){
                    enemy.gameObject.GetComponent<Vida_Inimiga>().TakeDamage(attackDamage);
                    collidedWithSomeone=true;
                } else {
                    enemy.gameObject.GetComponent<VidaBoss>().TakeDamage(attackDamage);
                    collidedWithSomeone=true;
                }
            }
            if (!collidedWithSomeone){audioPlayer.Play("AttackMiss");}
            else{audioPlayer.Play("SuccessHit");}
        }
    }

    private IEnumerator OnAttack() //implementa tempo entre ataques
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.2f);

        isAttacking = false;
    }

    void Parry()
    {
        if (!isParrying && !isAttacking)
        {
            anim.SetTrigger("Parry");
            StartCoroutine(OnParry()); //chama função que torna isParrying = true até que o limite de tempo entre parries passe;
        }
    }

    private IEnumerator OnParry() //implementa tempo entre parries
    {
        isParrying = true;

        yield return new WaitForSeconds(0.75f);

        isParrying = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "enemyIsAttacking") //note que para o parry funcionar o inimigo tem que ter essa tag quando ataca;
        { 
            if (isParrying) {
                if ((other.transform.position.x - transform.position.x) * transform.localScale.x > 0)
                {   
                    if (other.gameObject.GetComponent<Vida_Inimiga>() != null){
                        other.gameObject.GetComponent<Vida_Inimiga>().TakeDamage(parryDamage);
                        audioPlayer.Play("SuccessParry");
                    }
                    if (other.gameObject.GetComponent<VidaBoss>() != null){
                        other.gameObject.GetComponent<VidaBoss>().TakeDamage(parryDamage);
                        audioPlayer.Play("SuccessParry");
                    }
                    StartCoroutine(ParryAttack());
                }
                else {
                    healthScript.TomarDano(other.gameObject);
                }
            }
        }
    }

    private IEnumerator AttackFollowUp()
    {
        followUp = true;

        yield return new WaitForSeconds(0.8f);

        followUp = false;
    }

    public IEnumerator ParryAttack()
    {
        anim.SetTrigger("ParryAttack");
        UpdateParticles();
        parryParticles.Play();

        GetComponent<Player_Movement>().CanMove2 = false;
        
        yield return new WaitForSeconds(0.25f);

        GetComponent<Player_Movement>().CanMove2 = true;
    }

    private void UpdateParticles() {
        var vel = parryParticles.velocityOverLifetime;
        vel.x = new ParticleSystem.MinMaxCurve(4f * transform.localScale.x);

        var shape = parryParticles.shape;
        shape.position = new Vector3(0.72f * transform.localScale.x, -0.06f, 0f);
        shape.rotation = new Vector3(0f, 0f, 22.86f * transform.localScale.x);

    }
    
}
