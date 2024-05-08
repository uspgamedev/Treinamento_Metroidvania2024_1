using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFloating : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float frequency;

    Vector3 startPos;
    float sineOffset;

    void Awake()
    {
        startPos = transform.localPosition;
    }


    void Update()
    {
        sineOffset = amplitude * Mathf.Sin(frequency * Time.time) + startPos.y;
        transform.localPosition = new Vector3(startPos.x, sineOffset, 0f);
    }
}
