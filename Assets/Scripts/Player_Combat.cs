using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fonte: Otávio
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private LayerMask enemiesLayer; //qual a layer dos inimigos;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint; //define um ponto (um objeto) de referência para calcular o alcance do ataque;
    [SerializeField] private float attackRange; //range de ataque (perceba que ele é calculado a partir do attackPoint);
    [SerializeField] private int attackDamage; //dano de stun do ataque;
    [HideInInspector] public bool isAttacking = false;


    void Start()
    {
    }


    void Update()
    {
        //se J foi pressionado ataque;
        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack();
        }
    }

    void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(OnAttack()); //chama função que torna isAttacking = true até que o limite de tempo entre ataques passe;

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

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }
}
