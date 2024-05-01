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
    private enum CameraPosition{
        CENTERHV,
        CENTERV,
        CENTERH,
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        AUTO //Se AUTO, então a posição da camera é decidida com base na posição do player (geralmente não é problema, com exceção de salas em que as portas ficam em alturas diferentes.)
    }
    [SerializeField] private Vector3 nextPositionPlayer;
    // [SerializeField] private bool shouldFlip; // Deixando comentado apenas porque acho que isso não vai ser nescessário.
    // Mas qualquer coisa, tá ai um lembrete.
    [SerializeField] private CameraPosition nextPositionCamera = CameraPosition.CENTERH; //Em geral, deixar ela centralizada resolve na maioria dos casos.
    //A ideia é só ajustar a trava da camera em alguns mapas (Como o primeiro corredor comprido do tutorial por exemplo)
    [SerializeField] private float duracaoFade = 1f;
    [SerializeField] private PolygonCollider2D newCollider;
    [SerializeField] private LockAxis axisToLock = LockAxis.NONE;
    private bool encostou = false;
    private bool trocarCena = false;
    private int transFrame = 0;
    private GameObject blackFade;

    private bool defaded = false;
    private Image image;
    private Camera secondCamera;


    private void Start()
    {
        blackFade = GameObject.FindGameObjectWithTag("BlackFade");
        secondCamera = GameObject.FindGameObjectWithTag("SecondCamera").GetComponent<Camera>();
        image = blackFade.GetComponent<Image>();
        resetVariables();
    }

    private void FixedUpdate()
    {
        if (defaded)
        {
            transFrame++;
            float alpha = Mathf.Lerp(1f, 0f, (float)transFrame / (60 * duracaoFade));

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

              GameObject.FindGameObjectWithTag("Player").transform.position = nextPositionPlayer;

            Vector2 newCameraPosition;
              switch (nextPositionCamera)
                {
                    case CameraPosition.LEFT:
                        newCameraPosition = CalculateLeftPosition(newCollider.points);
                        break;
                    case CameraPosition.RIGHT:
                        newCameraPosition = CalculateRightPosition(newCollider.points);
                        break;
                    case CameraPosition.CENTERV: 
                        newCameraPosition = CalculateCenterPosition(newCollider.points, CameraPosition.CENTERV);
                        break;
                    case CameraPosition.CENTERH:
                        newCameraPosition = CalculateCenterPosition(newCollider.points, CameraPosition.CENTERH);
                        break;
                    case CameraPosition.CENTERHV:
                        newCameraPosition = CalculateCenterPosition(newCollider.points, CameraPosition.CENTERHV);
                        break;
                    case CameraPosition.BOTTOM:
                        newCameraPosition = CalculateBottomPosition(newCollider.points);
                        break;
                    case CameraPosition.TOP:
                        newCameraPosition = CalculateTopPosition(newCollider.points);
                        break;
                    default:
                        newCameraPosition = new Vector2(virtualCamera.transform.position.x, virtualCamera.transform.position.y);
                        break;
                }
                secondCamera.transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, virtualCamera.transform.position.z);
                virtualCamera.transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, virtualCamera.transform.position.z);
              resetVariables();
            }
        }
    }

    private Vector2 CalculateLeftPosition(Vector2[] points)
    {
        float leftmostX = Mathf.Infinity;
        foreach (Vector2 point in points)
        {
            if (point.x < leftmostX)
            {
                leftmostX = point.x;
            }
        }
        return new Vector2(leftmostX, 0f);
    }

    private Vector2 CalculateRightPosition(Vector2[] points)
    {
        float rightmostX = Mathf.NegativeInfinity;
        foreach (Vector2 point in points)
        {
            if (point.x > rightmostX)
            {
                rightmostX = point.x;
            }
        }
        return new Vector2(rightmostX, 0f);
    }

    private Vector2 CalculateBottomPosition(Vector2[] points)
    {
        float lowestY = Mathf.Infinity;
        foreach (Vector2 point in points)
        {
            if (point.y < lowestY)
            {
                lowestY = point.y;
            }
        }
        return new Vector2(0f, lowestY);
    }

    private Vector2 CalculateCenterPosition(Vector2[] points, CameraPosition centerType)
{
    switch (centerType)
    {
        case CameraPosition.CENTERV:
            float sumY = 0f;
            foreach (Vector2 point in points)
            {
                sumY += point.y;
            }
            return new Vector2(0f, sumY / points.Length);

        case CameraPosition.CENTERH:
            float sumX = 0f;
            foreach (Vector2 point in points)
            {
                sumX += point.x;
            }
            return new Vector2(sumX / points.Length, 0f);

        case CameraPosition.CENTERHV:
            Vector2 sum = Vector2.zero;
            foreach (Vector2 point in points)
            {
                sum += point;
            }
            return sum / points.Length;

        default:
            return Vector2.zero;
    }
}

    private Vector2 CalculateTopPosition(Vector2[] points)
    {
        float highestY = Mathf.NegativeInfinity;
        foreach (Vector2 point in points)
        {
            if (point.y > highestY)
            {
                highestY = point.y;
            }
        }
        return new Vector2(0f, highestY);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && image.color.a == 0)
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
