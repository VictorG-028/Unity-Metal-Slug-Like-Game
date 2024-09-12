using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAim : MonoBehaviour
{
    public float _timeToShoot = 3f; // Tempo de espera antes de atirar
    private float _timer = 0f; // Contador
    private bool _isTargetInSight = false; // Verifica se o jogador está na mira
    public GameObject _player;  // Referência ao jogador
    public float speed = 5f;   // Velocidade de movimento da mira

    private Vector2 targetPosition;  // Posição alvo (a posição do jogador)
    private Vector2 startPosition;   // Posição inicial (borda da tela)

    void Start()
    {
        // Encontra o jogador pela tag
        _player = GameObject.FindGameObjectWithTag("Player");

        if (_player != null)
        {
            targetPosition = _player.transform.position;  // Define a posição do jogador como alvo
        }

        // Define a posição inicial da mira (pode ser um ponto aleatório na borda da tela)
        startPosition = transform.parent.position; // Por exemplo, cima da tela
        transform.position = startPosition;  // Coloca a mira na posição inicial
    }

    void Update()
    {
        if (_player != null)
        {
            targetPosition = _player.transform.position;  // Atualiza a posição alvo

            // Move a mira em direção ao jogador usando um comportamento de steering
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica se o objeto é o jogador
        {
            if (!_isTargetInSight)
            {
                _isTargetInSight = true;
                _player = other.gameObject; // Armazena a referência ao jogador
                StartCoroutine(StartTimer());
            }
        }
    }

        // Corrotina para contar o tempo até o disparo
    IEnumerator StartTimer()
    {
        while (_timer < _timeToShoot)
        {
            _timer += Time.deltaTime;
            yield return null; // Espera o próximo frame
        }

        if (_isTargetInSight)
        {
            ShootPlayer(); // Chama a função de atirar
        }
    }

    void ShootPlayer()
    {
        if (_player != null)
        {
            _player.GetComponent<PlayerProperties>().TakeDamage(2); // Chama a função de TakeDamage do jogador
        }
        _timer = 0f; // Reseta o timer para a próxima vez
    }

}

