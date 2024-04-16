using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private LayerMask enemiesLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [HideInInspector] public bool isAttacking;


    void Start()
    {
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Attack();
            Debug.Log("Tavin");
        }
    }

    void Attack()
    {
        StartCoroutine(OnAttack());

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, enemiesLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.gameObject.GetComponent<Vida_Inimiga>().TakeDamage(attackDamage);
            Debug.Log("atacou");
        }
    }

    private IEnumerator OnAttack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }
}
