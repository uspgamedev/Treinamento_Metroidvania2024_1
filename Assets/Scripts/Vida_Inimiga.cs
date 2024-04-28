using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fonte: cópia do código do Otávio
public class Vida_Inimiga : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer; //qual a layer do player;
    [SerializeField] private int maxStun = 1; //número que define a quantidade de stun damage necessária para deixar o inimigo em stun;
    [SerializeField] private float stunTime = 0.5f; //a cada stunTime o currentStun do inimigo diminui em uma quantidade stunDecreaseRate;
    [SerializeField] private float stunDecreaseRate = 0.5f; //o quanto de currentStun o inimigo perde a cada stunTime segundos;
    [SerializeField] private float superMaxStun = 10; //hardcap do quanto que de stun que o player pode infligir no inimigo;
    [SerializeField] private float stunColorChangeTime = 0.5f; //intervalo de tempo entre mudanca de cores quando o inimigo esta estunado;
    [SerializeField] private float parryCircleRange = 2.0f; //o raio do circula que determina a area de parry do player;
    [SerializeField] private float currentStun; //a quantidade de stun que o inimigo apresenta
    //porque SerializeField e não public? public permite que as variáveis sejam acessadas por qualquer código e que elas sejam alteradas no inspetor,
    //SerializeField não permite que a variável seja acessada por qualquer código, porém permite que ela seja alterada no inspetor;
    private bool notStunned = true; //true se o inimigo está ativo (não está estunado);
    private bool canDecreaseStun = true; //true se a função StunDecrease deve ser chamada, para que a função seja chamada baseado no tempo e não no frame;
    private Color colorArchive; //salvando a cor do player para usar em StunColorChange;

    void Start()
    {
        colorArchive = GetComponent<SpriteRenderer>().color;
        currentStun = 0f;
    }

    void Update()
    {
        if(canDecreaseStun)
        {
            StartCoroutine(StunDecrease()); //chama a função que diminui currentStun a cada stunTime segundos;
        }

        if(!notStunned) //nota faz o hitkill do inimigo estunado, porem esse codigo talvez tenha que ser alterado a depender de como vamos lidar com a animacao do hitkill
        {
            Collider2D[] parryRange = Physics2D.OverlapCircleAll(GetComponent<Transform>().position, parryCircleRange, playerLayer);

            //se o player esta no range de parry entao é hitkill;
            foreach (Collider2D player in parryRange)
            {
                if (Input.GetKeyDown(KeyCode.K)) //tem que mudar esse botao para mudar o botao do parry
                { 
                    Destroy(gameObject); ;
                }
            }
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
        StartCoroutine(StunColorChange()); //faz o inimigo piscar
    }

        private IEnumerator StunColorChange()
    {
        while (currentStun > maxStun)
        {
            GetComponent<SpriteRenderer>().color = new Color(250, 250, 0); //mude esses parametros se quiser mudar a cor da piscada do inimigo
            yield return new WaitForSeconds(stunColorChangeTime);
            GetComponent<SpriteRenderer>().color = colorArchive;
            yield return new WaitForSeconds(stunColorChangeTime);
        }
    }
}
