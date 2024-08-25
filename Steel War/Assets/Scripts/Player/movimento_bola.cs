using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movimento_bola : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed = 5f;    // Velocidade de movimento
    public float jumpForce = 5f;    // Força do pulo
    private bool isGrounded;        // Verifica se está no chão

    private Rigidbody2D rb;

    public float speed = 5f;       // Velocidade de movimento horizontal

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Referência ao Rigidbody2D do personagem
    }

    void Update()
    {
        // Movimento horizontal
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // Pulo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
