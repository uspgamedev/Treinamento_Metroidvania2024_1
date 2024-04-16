using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vida_Inimiga : MonoBehaviour
{
    [SerializeField] private int maxStun = 1;
    [SerializeField] private float stunTime = 0.5f;
    [SerializeField] private float stunPercentage = 0.5f;
    [SerializeField] private float currentStun;
    private bool notStuned = true;

    void Start()
    {
        currentStun = 0f;
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentStun += damage;

        if (currentStun >= maxStun && notStuned)
        {
            EnemyStun();
            notStuned = false;
            StartCoroutine(StunWaitTime());
        }
    }

    private IEnumerator StunWaitTime()
    {
        yield return new WaitForSeconds(stunTime);
        notStuned = true;
        currentStun = Mathf.Floor(stunPercentage*maxStun);
    }

    private void EnemyStun() {

    }
}
