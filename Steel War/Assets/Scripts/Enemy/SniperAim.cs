using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAim : MonoBehaviour
{
     public GameObject player;  // Referência ao jogador
    public float speed = 5f;   // Velocidade de movimento da mira

    private Vector2 targetPosition;  // Posição alvo (a posição do jogador)
    private Vector2 startPosition;   // Posição inicial (borda da tela)

    void Start()
    {
        // Encontra o jogador pela tag
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            targetPosition = player.transform.position;  // Define a posição do jogador como alvo
        }

        // Define a posição inicial da mira (pode ser um ponto aleatório na borda da tela)
        startPosition = new Vector2(Random.Range(-10f, 10f), 5f); // Por exemplo, cima da tela
        transform.position = startPosition;  // Coloca a mira na posição inicial
    }

    void Update()
    {
        if (player != null)
        {
            targetPosition = player.transform.position;  // Atualiza a posição alvo

            // Move a mira em direção ao jogador usando um comportamento de steering
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }
}
