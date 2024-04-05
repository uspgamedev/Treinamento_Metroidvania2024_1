using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movement;
    [SerializeField] private Rigidbody2D rb;

    void Start()
    {
        
    }


    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
