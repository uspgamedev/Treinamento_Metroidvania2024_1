using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class PassageScript : MonoBehaviour
{
    private enum LockAxis{
        X,
        Y,
        XY,
        NONE
    }
    [SerializeField] private Vector3 nextPositionPlayer;
    // [SerializeField] private bool shouldFlip; // Deixando comentado apenas porque acho que isso não vai ser nescessário.
    // Mas qualquer coisa, tá ai um lembrete.
    [SerializeField] private float duracaoFade = 1f;
    [SerializeField] private PolygonCollider2D newCollider;
    [SerializeField] private LockAxis axisToLock = LockAxis.NONE;
    private bool encostou = false;
    private bool trocarCena = false;
    private int transFrame = 0;
    private GameObject blackFade;

    private bool defaded = false;

    private void Start()
    {
        blackFade = GameObject.FindGameObjectWithTag("BlackFade");
        resetVariables();
    }

    private void FixedUpdate()
    {
        if (defaded)
        {
            transFrame++;
            float alpha = Mathf.Lerp(1f, 0f, (float)transFrame / (60 * duracaoFade));

            Image image = blackFade.GetComponent<Image>();

            if (image != null)
            {
                image.color = new Color(0f, 0f, 0f, alpha);
            }

            if (alpha <= 0)
            {
                transFrame = 0;
                defaded = false;
            }
        }

        if (encostou && !trocarCena)
        {
            transFrame++;
            float alpha = Mathf.Lerp(0f, 1f, (float) transFrame / (60 * duracaoFade));

            Image image = blackFade.GetComponent<Image>();

            if (image != null)
            {
                image.color = new Color(0f, 0f, 0f, alpha);
            }

            if (alpha >= 1f)
            {
                trocarCena = true;
              GameObject worldBoundary = GameObject.FindGameObjectWithTag("WorldBoundary");
              PolygonCollider2D worldBoundaryCollider = worldBoundary.GetComponent<PolygonCollider2D>();

              if (worldBoundaryCollider != null && newCollider != null)
              {
                  worldBoundaryCollider.points = newCollider.points;
              }
              GameObject.FindGameObjectWithTag("Player").transform.position = nextPositionPlayer;
              GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
              CinemachineVirtualCamera virtualCamera = mainCamera.GetComponent<CinemachineVirtualCamera>();
              CinemachineFramingTransposer framingTranspose = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    switch (axisToLock) {
                        case LockAxis.X:
                            framingTranspose.m_DeadZoneWidth = 2f;
                            framingTranspose.m_DeadZoneHeight = 0f;
                            break;
                        case LockAxis.Y:
                            framingTranspose.m_DeadZoneHeight = 2f;
                            framingTranspose.m_DeadZoneWidth = 0f;
                            break;
                        case LockAxis.XY:
                            framingTranspose.m_DeadZoneWidth = 2f;
                            framingTranspose.m_DeadZoneHeight = 2f;
                            break;
                        default:
                            framingTranspose.m_DeadZoneWidth = 0f;
                            framingTranspose.m_DeadZoneHeight = 0f;
                            break;
                    }
              CinemachineConfiner2D confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
              confiner.InvalidateCache();

              resetVariables();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            encostou = true;
        }
    }

    private void resetVariables()
    {
        encostou = false;
        trocarCena = false;
        defaded = true;
        transFrame = 0;
    }
}
