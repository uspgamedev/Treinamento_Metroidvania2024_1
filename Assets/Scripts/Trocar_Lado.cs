using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Trocar_Lado : MonoBehaviour
{
    private GameObject[] listaA;
    private GameObject[] listaB;
    [SerializeField] private GameObject objetoATL;
    [SerializeField] private GameObject objetoBTL;
    [HideInInspector] public bool canChangeSides;
    private Collider2D coll;

    [SerializeField] float transTime;
    private Image whiteFade;
    private float alpha;
    private Health healthScript;
    [SerializeField] private SupportScript support;

    float min = 0f;
    float max = 1f;
    float t = 0f;

    private Animator anim;

    [SerializeField] private GameObject otherSideChanger;
    private GameObject player;
    private AudioManager audioPlayer;

    void Start()
    {
        anim = GetComponent<Animator>();

        whiteFade = GameObject.Find("WhiteFade").GetComponent<Image>();
        whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, 0f);

        canChangeSides = false;

        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        listaA = support.listaA;
        listaB = support.listaB;

        player = GameObject.Find("Player");
        audioPlayer = support.GetComponent<SupportScript>().getAudioManagerInstance();
    }

    void Update()
    {
        /*listaA = support.listaA;
        listaB = support.listaB;*/
        GameObject[] listaTemp = FindObjectsOfType<GameObject>(true);
        int ia = 0;
        int ib = 0;
        if (listaA[0] == null){
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
        }
        //muda de lado se o player esta dentro do local de mudar de lado e se o player pressionou a tecla E
        if(canChangeSides)
        {
            if (coll != null) 
            {
                if (coll.gameObject.tag == "Player") {
                    if (Input.GetKeyDown(KeyCode.E)) 
                    {
                        audioPlayer.Play("ChangeSideSFX");
                        StartCoroutine(ChangeSides());
                    }
                }
            }
        }
        if (support == null){
            support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        } else {
            if (support.toFadeWhite) {
                Fade(transTime, whiteFade, 1f, alpha);
                // healthScript.Fade(transTime, whiteFade, 1f, alpha);
            }
            else {
                if (whiteFade.color.a > 0f || t > 0f) {
                    whiteFade.color = new Color (0f, 0f, 0f, 0f);
                    alpha = 0f;
                    t = 0f;
                    min = 0f;
                    max = 1f;
                }
            }
        }
    }

    private IEnumerator ChangeSides() //habilita e desabilita objetos de acordo com o lado para o qual deve ser mudado
    {
        Debug.Log("Trocando");
        Debug.Log(objetoATL);
        Debug.Log(objetoBTL);
        Debug.Log(listaA[0]);
        Debug.Log(listaB[0]);

        support.toFadeWhite = true;
        canChangeSides = false;
        coll.GetComponent<Player_Movement>().canMove2 = false;

        bool activateB = false;
        if (objetoATL.activeInHierarchy) {
            activateB = true;
        }
        
        if (activateB) {
            objetoBTL.SetActive(true);
        }
        else {
            objetoATL.SetActive(true);
        }

        otherSideChanger.GetComponent<Trocar_Lado>().canChangeSides = false;

        anim.SetTrigger("Start");
        otherSideChanger.GetComponent<Animator>().SetTrigger("End");

        yield return new WaitForSeconds(transTime);

        foreach (GameObject objeto in listaA)
        {
            if (objeto != null)
            objeto.SetActive(!objeto.activeInHierarchy);
        }
        foreach (GameObject objeto in listaB)
        {
            if (objeto != null)
                objeto.SetActive(!objeto.activeInHierarchy);
        }

        player.transform.position = otherSideChanger.transform.position;
        

        yield return new WaitForSeconds(transTime/2);
        anim.SetTrigger("End");
        otherSideChanger.GetComponent<Animator>().SetTrigger("End");
        GameObject.Find("Player").GetComponent<Player_Movement>().canMove2 = true;
        yield return new WaitForSeconds(transTime/2);

        support.toFadeWhite = false;
        canChangeSides = true;
        if (activateB) {
            audioPlayer.SwitchSound("LadoA_BGM", "LadoB_BGM");
            objetoATL.SetActive(false);
        }
        else {
            audioPlayer.SwitchSound("LadoB_BGM", "LadoA_BGM");
            objetoBTL.SetActive(false);
        }

        if (support.ladoInicial == support.LadoInicialA){
        support.ladoInicial = support.LadoInicialB;
        } else {
        support.ladoInicial = support.LadoInicialA;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        support = GameObject.Find("ScriptsHelper").GetComponent<SupportScript>();
        canChangeSides = true;
        coll = collision;
        healthScript = coll.GetComponent<Health>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canChangeSides = false;
        coll = null;
    }

    public void Fade(float fadeDuration, Image image, float rgb, float a)
    {
        a = Mathf.Lerp(min, max, t);

        t += Time.deltaTime / fadeDuration;

        image.color = new Color(rgb, rgb, rgb, a);

        if (t >= 1f) {
            float temp = max;
            max = min;
            min = temp;
            t = 0f;
        }
    }
}
