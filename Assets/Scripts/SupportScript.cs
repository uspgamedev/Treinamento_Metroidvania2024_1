using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportScript : MonoBehaviour
{
    [SerializeField] private GameObject audioPrefab;
    private enum LadoInicial {
        A,
        B
    }

    [System.Flags]
    private enum Skills {
        Nothing = 0,
        Parry = 1 << 0,
        Dash = 1 << 1,
        Gancho = 1 << 2
    }

    [SerializeField] public GameObject[] listaA = new GameObject[100];
    [SerializeField] public GameObject[] listaB = new GameObject[100];

    [SerializeField] LadoInicial ladoInicial;
    [SerializeField] Skills skillsIniciais;
    [SerializeField] int frameRate = 60;

    [HideInInspector] public bool temParry = false;
    [HideInInspector] public bool temDash = false;
    [HideInInspector] public bool temGancho = false;

    [HideInInspector] public bool toFadeWhite = false;
    [HideInInspector] public Coroutine textCoroutine;

    [SerializeField] public int maxHealth = 3;

    [SerializeField] public List<int> healthIDToDeactivate;
    [SerializeField] public List<int> powerupIDToDeactivate;

    
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;

        GameObject t = GameObject.Find("CameraHandler");
        if (t != null) {
            GameObject canvas = t.transform.Find("Canvas").gameObject;
            canvas.GetComponent<Canvas>().enabled = true;
        }

        GameObject[] listaTemp = FindObjectsOfType<GameObject>(true);
        int ia = 0;
        int ib = 0;

        foreach (GameObject obj in listaTemp) {
            if (obj.CompareTag("LadoA")) {
                listaA[ia] = obj;
                ia++;
            }
            else if (obj.CompareTag("LadoB")) {
                listaB[ib] = obj;
                ib++;
            }
        }

        if (ladoInicial == LadoInicial.A) {
            foreach (GameObject objeto in listaA)
            {
                if (objeto != null)
                    objeto.SetActive(true);
            }
            foreach (GameObject objeto in listaB)
            {
                if (objeto != null)
                    objeto.SetActive(false);
            }
        }
        
        if (ladoInicial == LadoInicial.B) {
            foreach (GameObject objeto in listaB)
            {
                if (objeto != null)
                    objeto.SetActive(true);
            }
            foreach (GameObject objeto in listaA)
            {
                if (objeto != null)
                    objeto.SetActive(false);
            }
        }

        if ((skillsIniciais & Skills.Parry) == Skills.Parry) {
            temParry = true;
        }
        if ((skillsIniciais & Skills.Dash) == Skills.Dash) {
            temDash = true;
        }
        if ((skillsIniciais & Skills.Gancho) == Skills.Gancho) {
            temGancho = true;
        }
    }

    public AudioManager getAudioManagerInstance()
    {
        GameObject audioManagerObj = GameObject.Find("AudioManager");
        if (audioManagerObj != null) {
            return audioManagerObj.GetComponent<AudioManager>();
        } else {
            GameObject newAudioManager = Instantiate(audioPrefab, Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(newAudioManager);
            return newAudioManager.GetComponent<AudioManager>();
        }
    }
}