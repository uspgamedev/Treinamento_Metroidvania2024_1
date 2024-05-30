using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fonte: cópia do código do Otávio
public class Vida_Inimiga : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer; //qual a layer do player;
    [SerializeField] public int maxStun = 1; //número que define a quantidade de stun damage necessária para deixar o inimigo em stun;
    [SerializeField] private float stunTime = 0.5f; //a cada stunTime o currentStun do inimigo diminui em uma quantidade stunDecreaseRate;
    [SerializeField] private float stunDecreaseRate = 0.5f; //o quanto de currentStun o inimigo perde a cada stunTime segundos;
    [SerializeField] public float stunCooldownTime = 5f; //o tempo que leva para o inimigo parar de ficar estunado;
    // [SerializeField] private float superMaxStun = 10; //hardcap do quanto que de stun que o player pode infligir no inimigo;
    [SerializeField] public float currentStun; //a quantidade de stun que o inimigo apresenta
    //porque SerializeField e não public? public permite que as variáveis sejam acessadas por qualquer código e que elas sejam alteradas no inspetor,
    //SerializeField não permite que a variável seja acessada por qualquer código, porém permite que ela seja alterada no inspetor;
    [HideInInspector] public bool notStunned = true; //true se o inimigo está ativo (não está estunado);
    private bool canDecreaseStun = true; //true se a função StunDecrease deve ser chamada, para que a função seja chamada baseado no tempo e não no frame;

    private SimpleFlash flashScript;
    private float timeSinceHit = 100f;
    private GameObject yellowBar;
    private GameObject stunBar;

    [Header("Hp Drop")]
    [SerializeField] private GameObject hpCollect;
    [Range(0f, 1f)] [SerializeField] private float dropChance;

    void Start()
    {
        currentStun = 0f;

        stunBar = transform.GetChild(0).gameObject;
        yellowBar = transform.GetChild(0).GetChild(0).gameObject;

        flashScript = GetComponent<SimpleFlash>();
    }

    void Update()
    {
        if(canDecreaseStun && notStunned)
        {
            StartCoroutine(StunDecrease()); //chama a função que diminui currentStun a cada stunTime segundos;
        }

        if (timeSinceHit >= 6f) {
            stunBar.SetActive(false);
        }
        else {
            stunBar.SetActive(true);
        }

        timeSinceHit += Time.deltaTime;
    }

    public void TakeDamage(int damage) //da dano de stun ao inimigo
    {
        timeSinceHit = 0f;

        if (notStunned)
        {
            flashScript.Flash(Color.white);
            currentStun += damage;
        }

        if (!notStunned) {
            StartCoroutine(Die());
        }
        
        if (currentStun >= maxStun && notStunned)
        {
            currentStun = maxStun;
            notStunned = false;
            StartCoroutine(StunCooldown());
        }
    }

    private IEnumerator StunDecrease()
    {
        canDecreaseStun = false;

        //se passou stunTime segundos, diminua currentStun;
        if (currentStun > 0)
        {
            yield return new WaitForSeconds(stunTime);

            if (notStunned) {
                currentStun -= stunDecreaseRate;
            }
        }

        canDecreaseStun = true;
    }

    private IEnumerator StunCooldown() //para de diminuir o stun até o inimigo para de estar estunado
    {
        StartCoroutine(BarFlash());
        yield return new WaitForSeconds(stunCooldownTime);
        currentStun = 0f;
        notStunned = true;
    }


    private IEnumerator Die()
    {
        flashScript.Flash(Color.red);

        float willDrop = Random.Range(0f, 1f);

        if (willDrop <= dropChance) {
            Instantiate(hpCollect, transform.position + new Vector3(0f, 0f, 0f), Quaternion.identity);
        }

        yield return new WaitForSeconds(0.125f);

        gameObject.tag = "Morto";
        gameObject.SetActive(false);
    }

    private IEnumerator BarFlash()
    {
        while (!notStunned) {
            yellowBar.GetComponent<SimpleFlash>().Flash(Color.white);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
