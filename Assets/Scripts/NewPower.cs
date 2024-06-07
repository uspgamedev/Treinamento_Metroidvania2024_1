using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPower : MonoBehaviour
{
    private enum Skills {
        Parry,
        Dash,
        Gancho
    }

    private SupportScript support;
    private AudioManager audioPlayer;

    private bool canPickup = false;
    private SimpleFlash flashScript;

    [SerializeField] private Skills skillAcquired;
    [SerializeField] private GameObject skillText;

    [SerializeField] private int powerID;

    void Start()
    {
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        flashScript = transform.GetChild(0).GetComponent<SimpleFlash>();

        foreach (int id in support.powerupIDToDeactivate) {
            if (id == powerID) {
                gameObject.SetActive(false);
            }
        }
        audioPlayer = support.GetComponent<SupportScript>().getAudioManagerInstance();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") {
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
            StartCoroutine(GainSkill());
            canPickup = false;
        }
    }

    private IEnumerator GainSkill()
    {
        flashScript.Flash(Color.white);
        GameObject.Find("Player").GetComponent<SimpleFlash>().Flash(Color.white);

        support.powerupIDToDeactivate.Add(powerID);

        GetComponent<Collider2D>().enabled = false;

        if (skillAcquired == Skills.Parry) {
            support.temParry = true;
        }
        else if (skillAcquired == Skills.Dash) {
            support.temDash = true;
        }
        else if (skillAcquired == Skills.Gancho) {
            support.temGancho = true;
        }

        skillText.SetActive(true);
        audioPlayer.Play("PowerUP");

        yield return new WaitForSeconds(flashScript.duration);

        Destroy(transform.GetChild(0).gameObject);

        yield return new WaitForSeconds(8f);

        skillText.SetActive(false);
        Destroy(gameObject);
    }
}