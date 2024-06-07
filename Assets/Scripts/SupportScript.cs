using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SupportScript : MonoBehaviour
{
    [SerializeField] private GameObject audioPrefab;
    public enum LadoInicial {
        A,
        B
    }

    public LadoInicial LadoInicialA;
    public LadoInicial LadoInicialB;

    [System.Flags]
    private enum Skills {
        Nothing = 0,
        Parry = 1 << 0,
        Dash = 1 << 1,
        Gancho = 1 << 2
    }

    [SerializeField] public GameObject[] listaA = new GameObject[100];
    [SerializeField] public GameObject[] listaB = new GameObject[100];

    public LadoInicial ladoInicial;
    [SerializeField] Skills skillsIniciais;
    [SerializeField] int frameRate = 60;

    [HideInInspector] public bool temParry = false;
    [HideInInspector] public bool temDash = false;
    [HideInInspector] public bool temGancho = false;

    [HideInInspector] public bool toFadeWhite = false;
    [HideInInspector] public bool canChangeSides = true;
    [HideInInspector] public Coroutine textCoroutine;

    [SerializeField] public int maxHealth = 3;
    [HideInInspector] public int gemCount = 0;

    [HideInInspector] public List<int> healthIDToDeactivate;
    [HideInInspector] public List<int> powerupIDToDeactivate;
    [HideInInspector] public List<int> gemIDToDeactivate;

    [HideInInspector] public Vector3 lastRespawn;

    private GameObject listaATL;
    private GameObject listaBTL;

    [HideInInspector] public float respawn_x;
    [HideInInspector] public float respawn_y;
    [HideInInspector] public float respawn_z; 

    [HideInInspector] public int ultimoLado;
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        canChangeSides = true;

        ultimoLado = PlayerPrefs.GetInt("ultimo_lado", 1);

        if (ultimoLado == 1) {
            ladoInicial = LadoInicial.A;
        }
        else {
            ladoInicial = LadoInicial.B;
        }

        Debug.Log(ladoInicial);

        respawn_x = PlayerPrefs.GetFloat("pos_x", 0f); 
        respawn_y = PlayerPrefs.GetFloat("pos_y", 0f); 
        respawn_z = PlayerPrefs.GetFloat("pos_z", 0f); 

        lastRespawn = new Vector3(respawn_x, respawn_y, respawn_z);

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
            if (SceneManager.GetActiveScene().name.Contains("Mapa"))
                getAudioManagerInstance().FadeIn("LadoA_BGM");
            foreach (GameObject objeto in listaA)
            {
                if (objeto != null) {
                    if (objeto.tag != "Morto")
                        objeto.SetActive(true);
                }
            }
            foreach (GameObject objeto in listaB)
            {
                if (objeto != null) {
                    if (objeto.tag != "Morto")
                        objeto.SetActive(false);
                }
            }
        }
        
        if (ladoInicial == LadoInicial.B) {
            if (SceneManager.GetActiveScene().name.Contains("Mapa"))
                getAudioManagerInstance().FadeIn("LadoB_BGM");
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

        listaATL = GameObject.FindGameObjectWithTag("ListaATL");
        listaBTL = GameObject.FindGameObjectWithTag("ListaBTL");

        if (ladoInicial == LadoInicial.B) {
            if (listaATL != null){
                listaATL.SetActive(false);
            }
        }
        else {
            if (listaBTL != null){
                listaBTL.SetActive(false);
            }
        }

        LadoInicialA = LadoInicial.A;
        LadoInicialB = LadoInicial.B;        
    }

    public AudioManager getAudioManagerInstance()
    {
        if (AudioManager.Instance != null)
        {
            return AudioManager.Instance;
        }
        else
        {
            GameObject newAudioManager = Instantiate(audioPrefab, Vector3.zero, Quaternion.identity);
            return newAudioManager.GetComponent<AudioManager>();
        }
    }

    public void StartEnemyRespawn(GameObject objectToRespawn, float respawnTime, string originalTag) {
        StartCoroutine(RespawnEnemy(objectToRespawn, respawnTime, originalTag));
    }

    private IEnumerator RespawnEnemy(GameObject objectToRespawn, float respawnTime, string originalTag) {

        yield return new WaitForSeconds(respawnTime);

        objectToRespawn.tag = originalTag;
        string objectSide = objectToRespawn.transform.parent.gameObject.tag;

        if ((objectSide == "LadoA" && listaA[0].activeInHierarchy) || (objectSide == "LadoB" && listaB[0].activeInHierarchy)) {
            objectToRespawn.SetActive(true);
        }
    }

    private void SwapSideChangers() {
        listaATL.SetActive(!listaATL.activeInHierarchy);
        listaBTL.SetActive(!listaBTL.activeInHierarchy);
    }

    public void SetChange() {
        StartCoroutine(SetChangeSides());
    }

    private IEnumerator SetChangeSides() {
        canChangeSides = false;
        yield return new WaitForSeconds(2f);
        canChangeSides = true;
    }
}
