using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticles : MonoBehaviour
{
    public enum Enemy {
        Rat,
        Blob
    }

    [HideInInspector] public Enemy enemy;
    private ParticleSystem particle;

    void Awake()
    {
        StartCoroutine(LifeCycle());
    }


    private IEnumerator LifeCycle() {
        yield return new WaitForSeconds(0.05f);
        switch (enemy) {
            case Enemy.Rat:
                particle = transform.GetChild(0).GetComponent<ParticleSystem>();
                break;
            case Enemy.Blob:
                particle = transform.GetChild(1).GetComponent<ParticleSystem>();
                break;
        }
        particle.Play();
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
