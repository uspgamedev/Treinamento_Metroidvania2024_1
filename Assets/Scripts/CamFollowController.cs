using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowController : MonoBehaviour
{
    public enum CameraMovementType
    {
        HORIZONTAL,
        VERTICAL,
        FIXED
    }

    [SerializeField] private CameraMovementType movementType;
    [SerializeField] private float cameraSpeed = 5f;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 fixedPosition = new Vector3(0f, 0f, -10f); // Ainda não sei como isso vai rodar.

    private Vector2 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
        if (movementType == CameraMovementType.FIXED)
        {
            transform.position = fixedPosition;
        }
    }

    private void Update()
    {
        switch (movementType)
        {
            case CameraMovementType.HORIZONTAL:
                HandleHORIZONTALMovement();
                break;
            case CameraMovementType.VERTICAL:
                HandleVerticalMovement();
                break;
            case CameraMovementType.FIXED:
                break;
        }
    }

    private void HandleHORIZONTALMovement()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);
        }
    }

    private void HandleVerticalMovement()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);
        }
    }
}
