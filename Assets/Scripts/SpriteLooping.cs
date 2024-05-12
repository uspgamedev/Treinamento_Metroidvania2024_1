using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;
using DG.Tweening;

public class SpriteLooping : MonoBehaviour
{
    private enum LoopAxis{
        X,
        Y,
    }

    private GameObject[] listaA;
    private GameObject[] listaB;
    [SerializeField] float transTime;
    private Image blackFade;
    public bool allowSelection = false;

    [SerializeField] private GameObject loopingObject;
    [SerializeField] private float LOOP_SPEED = 1f;
    [SerializeField] private LoopAxis axisToLoop = LoopAxis.Y;
    [SerializeField] private Vector2 loopBoundary; //Só pra poder deixar bem evidente o limite em cada eixo, mesmo que não vá usar os dois limites ao mesmo tempo.
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = loopingObject.transform.position;   

        blackFade = GameObject.Find("BlackFade").GetComponent<Image>();
        blackFade.DOFade(0f, transTime);

        listaA = GameObject.FindObjectOfType<SupportScript>().GetComponent<SupportScript>().listaA;
        listaB = GameObject.FindObjectOfType<SupportScript>().GetComponent<SupportScript>().listaB; 
    }
    void FixedUpdate()
    {
        loopingObject.transform.position = updatePosition(loopingObject.transform.position);
    }


    private Vector3 updatePosition(Vector3 vector){
        switch(axisToLoop){
            case LoopAxis.Y:
                if (vector.y <= loopBoundary.y){
                    vector.y+=LOOP_SPEED;
                    return vector;
                }
                break;
            case LoopAxis.X:
                if (vector.x >= loopBoundary.x){
                    vector.x+=LOOP_SPEED;
                    return vector;
                }
                break;
        }
        StartCoroutine(ChangeSides());  // Só vai chegar neste ponto, quando o loop chegar no limite, então tá safe
                                        // Isso também serve como uma solução bem vagabunda pra resolver
                                        // O problema da sincronização do topo com o fundo do tileset.
        return initialPosition;

    }
    private IEnumerator ChangeSides()
    {
        blackFade.DOFade(1f, transTime);

        yield return new WaitForSeconds(transTime);

        // Change sides
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

        blackFade.DOFade(0f, transTime);

        yield return new WaitForSeconds(transTime);

        allowSelection = true;
    }
}
