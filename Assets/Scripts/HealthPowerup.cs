using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthPowerup : MonoBehaviour
{
    private Health healthScript;
    private SupportScript support;

    [SerializeField] private Color flashColor;
    [SerializeField] public GameObject hpObject;
    private bool canPickup = false;

    [SerializeField] public int healthID;

    void Start()
    {
        if (GameObject.Find("Player")!= null)
            healthScript = GameObject.Find("Player").GetComponent<Health>();
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();

        foreach (int id in support.healthIDToDeactivate) {
            if (id == healthID) {
                hpObject.SetActive(false);
            }
        }
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
        if (Input.GetKeyDown(KeyCode.E) && canPickup && hpObject.gameObject.activeInHierarchy) {
            StartCoroutine(HpCollect());
            canPickup = false;
        }
    }

    IEnumerator HpCollect()
    {
        if (healthScript!=null){
            healthScript.HealthUp();
            healthScript.GetComponent<SimpleFlash>().Flash(Color.green);
        }
        hpObject.GetComponent<SimpleFlash>().Flash(Color.green);

        support.maxHealth++;

        yield return new WaitForSeconds(0.125f);

        support.healthIDToDeactivate.Add(healthID);
        hpObject.SetActive(false);
    }
}