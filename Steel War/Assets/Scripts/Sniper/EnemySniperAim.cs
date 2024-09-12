using System.Collections;
using UnityEngine;

public class EnemySniperAim : MonoBehaviour
{
    PlayerProperties playerProps;
    public GameObject player;  // Referência ao jogador
    public float maxSpeed = 8f;   // Velocidade máxima da mira
    public float minSpeed = 2f;   // Velocidade mínima ao se aproximar do jogador
    public float maxForce = 0.2f; // Força máxima aplicada para mudar a direção
    public float slowDownRadius = 5f;  // Raio dentro do qual a velocidade será reduzida
    public float timeToShoot = 5f;     // Tempo necessário para disparar após contato

    private Vector2 velocity;     // Velocidade atual da mira
    private Vector2 acceleration; // Aceleração da mira
    private Vector2 targetPosition;  // Posição alvo (a posição do jogador)
    
    private float contactTime = 0f;   // Tempo de contato com o jogador
    private bool isInContact = false; // Verifica se está em contato com o jogador

    void Start()
    {
        // Encontra o jogador pela tag
        player = GameObject.FindGameObjectWithTag("Player");
        playerProps=player.GetComponent<PlayerProperties>();
        
        if (player != null)
        {
            targetPosition = player.transform.position;  // Define a posição do jogador como alvo
        }

        // Define a posição inicial da mira como a posição do sniper (pai)
        transform.position = transform.parent.position;  
        velocity = Vector2.zero;
        acceleration = Vector2.zero;
    }

    void Update()
    {
        if (player != null)
        {
            targetPosition = player.transform.position;  // Atualiza a posição alvo

            // Calcula a distância até o jogador
            float distance = Vector2.Distance(transform.position, targetPosition);

            // Define a velocidade com base na distância (diminui a velocidade quando estiver perto)
            float speed = maxSpeed;
            if (distance < slowDownRadius)
            {
                speed = Mathf.Lerp(minSpeed, maxSpeed, distance / slowDownRadius);
            }

            // Calcula a força de steering para perseguir o jogador
            Vector2 desired = (targetPosition - (Vector2)transform.position).normalized * speed;
            Vector2 steer = desired - velocity; // Steering = direção desejada - velocidade atual
            steer = Vector2.ClampMagnitude(steer, maxForce); // Limita a força de steering
            ApplyForce(steer); // Aplica a força de steering

            // Atualiza a física da mira
            velocity += acceleration;
            velocity = Vector2.ClampMagnitude(velocity, maxSpeed); // Limita a velocidade máxima
            transform.position += (Vector3)velocity * Time.deltaTime; // Move a mira

            acceleration *= 0; // Reseta a aceleração para o próximo frame

            // Verifica o tempo de contato com o jogador
            if (isInContact)
            {
                Debug.Log("Em contato");
                contactTime += Time.deltaTime;
                if (contactTime >= timeToShoot)
                {
                    playerProps.TakeDamage(2);
                    contactTime = 0f;  // Reseta o tempo após atirar
                }
            }
            else
            {
                contactTime = 0f; // Reseta o tempo se não estiver em contato
            }
        }
    }

    // Aplica uma força à aceleração
    void ApplyForce(Vector2 force)
    {
        acceleration += force;
    }

    // Método chamado quando a mira entra em contato com o jogador
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            isInContact = true;  // Marca que a mira está em contato com o jogador
        }
    }

    // Método chamado quando a mira sai de contato com o jogador
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            isInContact = false;  // Marca que a mira saiu de contato com o jogador
        }
    }

}
