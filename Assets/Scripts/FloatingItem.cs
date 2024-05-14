using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFloating : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float frequency;

    Vector3 startPos;
    float sineOffset;

    float currentTime = 0f;

    [HideInInspector] public bool canUpdate = true;

    void Awake()
    {
        startPos = transform.localPosition;
    }


    void Update()
    {
        if (canUpdate) {
            currentTime += Time.deltaTime;
            sineOffset = amplitude * Mathf.Sin(frequency * currentTime) + startPos.y;
            // sineOffset = amplitude * Mathf.Sin(frequency * Time.time) + startPos.y;
            transform.localPosition = new Vector3(startPos.x, sineOffset, 0f);
        }
    }
}
