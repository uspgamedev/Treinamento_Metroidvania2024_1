using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fonte: cópia do código do Otávio
public class Vida_Inimiga : MonoBehaviour
{
    [SerializeField] private int maxStun = 1; //número que define a quantidade de stun damage necessária para deixar o inimigo em stun;
    [SerializeField] private float stunTime = 0.5f; //a cada stunTime o currentStun do inimigo diminui em uma quantidade stunDecreaseRate;
    [SerializeField] private float stunDecreaseRate = 0.5f; //o quanto de currentStun o inimigo perde a cada stunTime segundos;
    [SerializeField] private float superMaxStun = 10; //hardcap do quanto que de stun que o player pode infligir no inimigo;
    [SerializeField] private float currentStun; //a quantidade de stun que o inimigo apresenta
    //porque SerializeField e não public? public permite que as variáveis sejam acessadas por qualquer código e que elas sejam alteradas no inspetor,
    //SerializeField não permite que a variável seja acessada por qualquer código, porém permite que ela seja alterada no inspetor;
    private bool notStunned = true; //true se o inimigo está ativo (não está estunado);
    private bool canDecreaseStun = true; //true se a função StunDecrease deve ser chamada, para que a função seja chamada baseado no tempo e não no frame;

    void Start()
    {
        currentStun = 0f;
    }

    void Update()
    {
        if(canDecreaseStun)
        {
        StartCoroutine(StunDecrease()); //chama a função que diminui currentStun a cada stunTime segundos;
        }
    }

    public void TakeDamage(int damage) //da dano de stun ao inimigo
    {
        if (currentStun <= superMaxStun)
        {
            currentStun += damage;
        }

        if (currentStun >= maxStun && notStunned)
        {
            EnemyStun(); //a ser implementado
            notStunned = false;
        }
    }

    private IEnumerator StunDecrease()
    {
        canDecreaseStun = false;
        //se passou stunTime segundos, diminua currentStun;
        if (currentStun > 0)
        {
            yield return new WaitForSeconds(stunTime);
            currentStun -= stunDecreaseRate;

        }

        if (currentStun < maxStun)
        {
            notStunned = true;
        }

        canDecreaseStun = true;
    }

    private void EnemyStun() //a ser implementado
    {
    }
}
