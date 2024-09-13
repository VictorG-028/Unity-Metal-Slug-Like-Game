using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemySniperAim : MonoBehaviour
{
    [SerializeField] AudioSource shootingClip = null;
    [SerializeField] Animator sniperAnimator = null;
    [SerializeField] PlayerProperties playerProps = null;
    [SerializeField] SpriteRenderer spriteRenderer = null;

    private GameObject player = null;  // Referência ao jogador
    private float maxSpeed;   // Velocidade máxima da mira
    private float minSpeed;   // Velocidade mínima ao se aproximar do jogador
    private float maxForce; // Força máxima aplicada para mudar a direção
    private float slowDownRadius;  // Raio dentro do qual a velocidade será reduzida
    private float timeToShoot;     // Tempo necessário para disparar após contato

    private Vector2 velocity;     // Velocidade atual da mira
    private Vector2 acceleration; // Aceleração da mira
    private Vector2 targetPosition;  // Posição alvo (a posição do jogador)
    
    private float contactTime = 0f;   // Tempo de contato com o jogador
    private bool isInContact = false; // Verifica se está em contato com o jogador
    private float speed = 0;
    private float distance = 0;
    private bool isPlayerOnLeft = false;
    private float maxPursuitDistance = 18;

    void OnValidate()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (!shootingClip){
            shootingClip = GetComponent<AudioSource>();
        }

        if(!sniperAnimator){
            sniperAnimator = transform.parent.GetComponent<Animator>();
        }
        
        if(!playerProps){
            playerProps = player.GetComponent<PlayerProperties>();
        }

        if(!spriteRenderer){
            spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        }
        
    }


    void Start()
    {
        maxSpeed = 3f;   // Velocidade máxima da mira
        minSpeed = 1f;   // Velocidade mínima ao se aproximar do jogador
        maxForce = 0.2f; // Força máxima aplicada para mudar a direção
        slowDownRadius = 5f;  // Raio dentro do qual a velocidade será reduzida
        timeToShoot = 5f;     // Tempo necessário para disparar após contato
        
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
        isPlayerOnLeft = transform.parent.position.x > player.transform.position.x;
        if(isPlayerOnLeft){
            spriteRenderer.flipX = true;
        }
        else{
            spriteRenderer.flipX = false;
        }
        

        targetPosition = player.transform.position+Vector3.up;  // Atualiza a posição alvo
        distance = Vector2.Distance(transform.position, targetPosition);

        if(distance > maxPursuitDistance){
            transform.position = transform.parent.position;
            return;
        }

        UpdateSpriteByDistance(distance);

        // Define a velocidade com base na distância (diminui a velocidade quando estiver perto)
        speed = maxSpeed;
        if (distance < slowDownRadius)
        {
            speed = Mathf.Lerp(minSpeed, maxSpeed, distance / slowDownRadius);
        }

        // Calcula a força de steering para perseguir o jogador
        Vector2 desired = (targetPosition - (Vector2)transform.position).normalized * speed;
        Vector2 steer = desired - velocity; // Steering = direção desejada - velocidade atual
        steer = Vector2.ClampMagnitude(steer, maxForce); // Limita a força de steering
        acceleration += steer; // Aplica a força de steering

        // Atualiza a física da mira
        velocity += acceleration;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed); // Limita a velocidade máxima
        transform.position += (Vector3)velocity * Time.deltaTime; // Move a mira

        acceleration *= 0; // Reseta a aceleração para o próximo frame

        // Verifica o tempo de contato com o jogador
        if (isInContact)
        {
            //Debug.Log("Em contato");
            contactTime += Time.deltaTime;
            if (contactTime >= timeToShoot)
            {
                playerProps.TakeDamage(2);
                contactTime = 0f;  // Reseta o tempo após atirar
                transform.position=transform.parent.position;
                shootingClip.Play();
            }
        }
        else
        {
            contactTime = 0f; // Reseta o tempo se não estiver em contato
        }
    }

    // Método chamado quando a mira entra em contato com o jogador
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Testando");
        if (other.gameObject.CompareTag("Player"))
        {
            isInContact = true;  // Marca que a mira está em contato com o jogador
        }
    }

    // Método chamado quando a mira sai de contato com o jogador
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isInContact = false;  // Marca que a mira saiu de contato com o jogador
        }
    }

    private void UpdateSpriteByDistance(float distance){
        if(distance <5){
            sniperAnimator.Play("standing4");
        }
        else if (distance <8){
            sniperAnimator.Play("standing3");
        }
        else if (distance <11){
            sniperAnimator.Play("standing2");
        }
        else if (distance <14){
            sniperAnimator.Play("standing1");
        }
        else{
            sniperAnimator.Play("Idle");
        }
    }

}