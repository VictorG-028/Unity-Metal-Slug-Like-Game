using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlataform : MonoBehaviour
{
    public float moveSpeed = 2f;            // Velocidade de movimento da plataforma
    public float moveDistance = 3f;         // Distância que a plataforma irá percorrer
    public bool isVertical = false;         // Flag para definir a direção do movimento

    private Vector3 startPosition;
    private Rigidbody2D rb;

    void Start()
    {
        // Armazena a posição inicial da plataforma
        startPosition = transform.position;

        // Obtém o Rigidbody2D do objeto
        rb = GetComponent<Rigidbody2D>();

        // Configura o Rigidbody2D para ser cinemático, pois queremos controlar o movimento manualmente
        rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        // Calcula a nova posição da plataforma
        Vector3 newPosition = startPosition;

        if (isVertical)
        {
            // Movimento para cima e para baixo
            newPosition.y += Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        }
        else
        {
            // Movimento para a esquerda e direita
            newPosition.x += Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        }

        // Atualiza a posição da plataforma através do Rigidbody2D
        rb.MovePosition(newPosition);
    }
}
