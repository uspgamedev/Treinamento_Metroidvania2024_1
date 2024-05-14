using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.PlayerLoop;

//Fonte: Otávio
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private LayerMask enemiesLayer; //qual a layer dos inimigos;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint; //define um ponto (um objeto) de referência para calcular o alcance do ataque;
    [SerializeField] private float attackRange; //range de ataque (perceba que ele é calculado a partir do attackPoint);
    [SerializeField] private int attackDamage; //dano de stun do ataque;
    [SerializeField] private int parryDamage;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isParrying = false;

    private Animator anim;
    private bool followUp = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        //se J foi pressionado ataque;
        if (Input.GetKeyDown(KeyCode.J)) //tem que mudar esse botao para mudar o botao do ataque
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.K) && !GetComponent<Player_Movement>().canGancho) //tem que mudar esse botao para mudar o botao do parry
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

            //para cada inimigo no range de ataque chame TakeDamage;
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.gameObject.GetComponent<Vida_Inimiga>().TakeDamage(attackDamage);
            }
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
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, enemiesLayer);
            bool attack = false;

            //para cada inimigo no range de ataque chame TakeDamage;
            foreach (Collider2D enemy in hitEnemies)
            {
                if (!enemy.GetComponent<Vida_Inimiga>().notStunned && ((transform.position.x - enemy.gameObject.transform.position.x) * transform.localScale.x) <= 0) {
                    enemy.gameObject.GetComponent<Vida_Inimiga>().TakeDamage(attackDamage);
                    attack = true;
                }
            }

            if (!attack) {
                anim.SetTrigger("Parry");
                StartCoroutine(OnParry()); //chama função que torna isParrying = true até que o limite de tempo entre parries passe;
            }
            else {
                StartCoroutine(ParryAttack());
            }
        
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
            if (isParrying)
            {
                other.gameObject.GetComponent<Vida_Inimiga>().TakeDamage(parryDamage);
                StartCoroutine(ParryAttack());
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
        GetComponent<Player_Movement>().canMove2 = false;

        yield return new WaitForSeconds(0.25f);

        GetComponent<Player_Movement>().canMove2 = true;
    }
    
}
