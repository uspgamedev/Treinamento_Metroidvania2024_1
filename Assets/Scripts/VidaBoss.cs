using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaBoss : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer; //qual a layer do player;
    [SerializeField] public int maxStun = 1; //n�mero que define a quantidade de stun damage necess�ria para deixar o inimigo em stun;
    [SerializeField] private float stunTime = 0.5f; //a cada stunTime o currentStun do inimigo diminui em uma quantidade stunDecreaseRate;
    [SerializeField] private float stunDecreaseRate = 0.5f; //o quanto de currentStun o inimigo perde a cada stunTime segundos;
    [SerializeField] public float stunCooldownTime = 5f; //o tempo que leva para o inimigo parar de ficar estunado;
    // [SerializeField] private float superMaxStun = 10; //hardcap do quanto que de stun que o player pode infligir no inimigo;
    [SerializeField] public float currentStun; //a quantidade de stun que o inimigo apresenta
    //porque SerializeField e n�o public? public permite que as vari�veis sejam acessadas por qualquer c�digo e que elas sejam alteradas no inspetor,
    //SerializeField n�o permite que a vari�vel seja acessada por qualquer c�digo, por�m permite que ela seja alterada no inspetor;
    [HideInInspector] public bool notStunned = true; //true se o inimigo est� ativo (n�o est� estunado);
    private bool canDecreaseStun = true; //true se a fun��o StunDecrease deve ser chamada, para que a fun��o seja chamada baseado no tempo e n�o no frame;

    private SimpleFlash flashScript;
    private float timeSinceHit = 100f;
    private GameObject yellowBar;
    private GameObject stunBar;

    private GameObject block;

    // Start is called before the first frame update
    void Start()
    {
        currentStun = 0f;
        flashScript = GetComponent<SimpleFlash>();
        block = GameObject.Find("Block");
    }

    // Update is called once per frame
    void Update()
    {
        if(canDecreaseStun && notStunned)
        {
            StartCoroutine(StunDecrease()); //chama a fun��o que diminui currentStun a cada stunTime segundos;
        }



        timeSinceHit += Time.deltaTime;
    }

    public void TakeDamage(int damage) //da dano de stun ao inimigo
    {
        timeSinceHit = 0f;

        flashScript.Flash(Color.white);

        if (notStunned)
        {
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

    private IEnumerator StunCooldown() //para de diminuir o stun at� o inimigo para de estar estunado
    {
        yield return new WaitForSeconds(stunCooldownTime);
        currentStun = 0f;
        notStunned = true;
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.125f);

        currentStun = 0f;
        notStunned = true;
        timeSinceHit = 50f;

        gameObject.layer = LayerMask.NameToLayer("Ground");
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0f;

        GetComponent<BossAi>().toDie = true;

        Destroy(block);
    }
}
