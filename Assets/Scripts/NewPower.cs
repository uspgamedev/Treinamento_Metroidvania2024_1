using System.Collections;
using UnityEngine;

public class NewPower : MonoBehaviour
{
    private enum Skills{Parry, Dash, Gancho}
    [SerializeField] private Skills skillAcquired;
    [SerializeField] private GameObject skillText;
    [SerializeField] private int powerID;
    private SupportScript support;
    private AudioManager audioPlayer;
    private SimpleFlash flashScript;
    private bool canPickup = false;
    private float pickupTime;
    private void Start()
    {
        support = FindObjectOfType<SupportScript>();
        if (support == null)
        {
            Debug.LogError("SupportScript is missing!");
            return;
        }

        flashScript = transform.GetChild(0).GetComponent<SimpleFlash>();
        if (flashScript == null)
        {
            Debug.LogError("SimpleFlash component not found in child object!");
            return;
        }

        foreach (int id in support.powerupIDToDeactivate)
        {
            if (id == powerID)
            {
                gameObject.SetActive(false);
                break;
            }
        }

        audioPlayer = support.GetAudioManagerInstance();
        if (audioPlayer == null)
        {
            Debug.LogWarning("AudioManager not found in SupportScript!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickup = true;
            pickupTime = Time.realtimeSinceStartup;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickup = false;
        }
    }

    private void Update()
    {
        if (canPickup && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(GainSkill());
        }
    }

    private IEnumerator GainSkill()
    {
        flashScript.Flash(Color.white);
        var playerFlashScript = GameObject.Find("Player")?.GetComponent<SimpleFlash>();
        playerFlashScript?.Flash(Color.white);

        support.powerupIDToDeactivate.Add(powerID);
        GetComponent<Collider2D>().enabled = false;

        switch (skillAcquired)
        {
            case Skills.Parry:
                support.temParry = true;
                break;
            case Skills.Dash:
                support.TemDash = true;
                break;
            case Skills.Gancho:
                support.TemGancho = true;
                break;
        }

        if (audioPlayer != null)
        {
            audioPlayer.Play("PowerUP");
        }
        else
        {
            Debug.LogWarning("AudioManager not found in SupportScript!");
        }

        skillText.SetActive(true);
        yield return new WaitForSeconds(flashScript.duration);

        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }

        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            yield return null; // Wait for one frame
            elapsedTime = Time.realtimeSinceStartup - pickupTime;
        }

        skillText.SetActive(false);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
