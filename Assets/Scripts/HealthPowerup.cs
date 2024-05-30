using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerup : MonoBehaviour
{
    private Health healthScript;
    [SerializeField] private Color flashColor;
    [SerializeField] private GameObject hpObject;
    private bool canPickup = false;

    void Start()
    {
        if (GameObject.Find("Player")!= null)
            healthScript = GameObject.Find("Player").GetComponent<Health>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && hpObject.gameObject.activeInHierarchy) {
            canPickup = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player") {
            canPickup = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPickup) {
            StartCoroutine(HpCollect());
            canPickup = false;
        }
    }

    IEnumerator HpCollect()
    {
        if (healthScript!=null){
            healthScript.HealthUp();
            healthScript.GetComponent<SimpleFlash>().Flash(flashColor);
        }
        hpObject.GetComponent<SimpleFlash>().Flash(flashColor);

        yield return new WaitForSeconds(0.125f);

        hpObject.SetActive(false);
    }
}
