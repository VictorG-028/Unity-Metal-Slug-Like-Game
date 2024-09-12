using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BasicEnemyBehavior : MonoBehaviour
{
    [SerializeField] EnemyProperties enemyProps = null;
    [SerializeField] BoxCollider2D boxCollider = null;
    [SerializeField] Rigidbody2D rb = null;
    [SerializeField] Transform playerTransform = null;
    [SerializeField] Transform gunBarrelPoint = null;
    [SerializeField] Sprite bulletSprite = null;
    [SerializeField] Animator animator = null;
    [SerializeField] SpriteRenderer spriteRenderer = null;


    private void OnValidate()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (!enemyProps) { enemyProps = GetComponent<EnemyProperties>(); }
        if (!boxCollider) { boxCollider = GetComponent<BoxCollider2D>(); }
        if (!rb) { rb = GetComponent<Rigidbody2D>(); }
        if (!playerTransform && player) { playerTransform = player.transform; }
        if (!gunBarrelPoint) { gunBarrelPoint = transform.Find("Gun Barrel Point"); }
        if (!bulletSprite) { bulletSprite = Resources.Load<Sprite>("TODO"); }
        if (!animator) { animator = GetComponent<Animator>(); }
        if (!spriteRenderer) { spriteRenderer = GetComponent<SpriteRenderer>(); }
    }

    // Public parameters
    [Range(0.0f, 10.0f)] public float delayBetweenAttacks = 6.0f;
    [Range(0.0f, 15.0f)] public float minCloseDistance = 10.5f;
    [Range(1.0f, 15.0f)] public float bulletVelocidy = 10.0f;
    [Range(3.0f, 100.0f)] public float bulletDuration = 10.0f;
    [Range(0, 10)] public int bulletDamage = 1;
    [Range(1.0f, 10.0f)] public float walkingSpeed = 5f;

    // Control
    //private bool isBusyDoingAction = false;
    //private bool hasTriggerredAnimation = false;
    private bool isAwaitingInitialAnimationFinish = true;
    private bool isShooting = false;
    private bool isWalking = false;
    private bool isGoingToWalk = false;
    private float distanceToPlayer = 0.0f;
    //private readonly object _lock = new();
    private AnimatorStateInfo info;
    private Vector3 directionAwayFromPlayer =  Vector3.zero;
    private Vector3 lookDirection = Vector3.zero;
    private GameObject bulletObject = null;
    private SpriteRenderer bulletSpriteRenderer = null;
    private BoxCollider2D bulletCollider = null;
    private Rigidbody2D bulletRigidbody = null;
    private AttackBehaviour bulletAttackBehaviour = null;


    void Update()
    {
        distanceToPlayer = CalculateDistanceToPlayer();
        isGoingToWalk = distanceToPlayer < minCloseDistance;
        info = animator.GetCurrentAnimatorStateInfo(0);
        //isShooting = info.IsName("Shooting");
        //isWalking = info.IsName("Walking") || info.IsName("Pulling In Gun");
        //isStandingStill = info.IsName("PointingGunStanding");

        //print($"Distância: {distanceToPlayer} < {minCloseDistance} -> {isGoingToWalk}");
        //print($"Lenght da animação: {info.length} | ");
        //print($"{enemyProps.canAttack} && {info.IsName("Pointing Gun Standing")} {enemyProps.canAttack && info.IsName("Pointing Gun Standing")}");

        if (isAwaitingInitialAnimationFinish)
        {
            isAwaitingInitialAnimationFinish = !info.IsName("Pointing Gun Standing");
        }
        else if (isShooting)
        {
            StopShooting();
        }
        else if (isGoingToWalk)
        {
            WalkOpositeDirection();
        }
        else if (isWalking)
        {
            StopWalking();
        }
        else if (enemyProps.canAttack && info.IsName("Pointing Gun Standing"))
        {
            //print($"Inimigo {this.name} deve atirar");
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        enemyProps.canAttack = false;
        spriteRenderer.flipX = !IsPlayerOnLeftSide(); // Make sprite face the oposite side of the player
        isShooting = true;
        animator.SetBool("isShooting", true);
        //animator.SetTrigger("isShootingTrigger");
        //animator.Play("Shooting 0");

        lookDirection = CalculateLookDirection();

        bulletObject = new("Bullet") { tag = "Attack" };
        bulletObject.transform.position = gunBarrelPoint.position;

        // Adiciona um sprite renderer com um sprite
        bulletSpriteRenderer = bulletObject.AddComponent<SpriteRenderer>();
        bulletSpriteRenderer.sprite = bulletSprite;
        bulletObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Adiciona um colider e rigidyBody
        bulletCollider = bulletObject.AddComponent<BoxCollider2D>();
        bulletCollider.size = new Vector2(1.0f, 1.0f); // Largura, Altura
        bulletCollider.isTrigger = true;
        bulletRigidbody = bulletObject.AddComponent<Rigidbody2D>();
        bulletRigidbody.bodyType = RigidbodyType2D.Dynamic;
        bulletRigidbody.velocity = lookDirection * bulletVelocidy;
        bulletRigidbody.gravityScale = 0;

        // Adiciona lógica de colisão e dano
        bulletAttackBehaviour = bulletObject.AddComponent<AttackBehaviour>();
        bulletAttackBehaviour.damage = bulletDamage;
        bulletAttackBehaviour.isShootedByEnemy = true;

        StartCoroutine(CanAttackAfterDelay(delayBetweenAttacks));
        Destroy(bulletObject, bulletDuration);
    }

    private void WalkOpositeDirection()
    {
        if (info.IsName("Pointing Gun Standing"))
        {
            isWalking = true;
            animator.SetBool("isWalking", true);
            //animator.Play("Pulling In Gun 0");
            //animator.Play("Walking 0");
        }

        if (info.IsName("Walking"))
        {
            // Direção oposta ao jogador no eixo x
            directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;
            directionAwayFromPlayer = new Vector3(directionAwayFromPlayer.x, 0, 0);

            spriteRenderer.flipX = IsPlayerOnLeftSide();

            transform.position += Time.deltaTime * walkingSpeed * directionAwayFromPlayer;
        }
    }

    private void StopWalking()
    {
        //animator.Play("PointingGunStanding");
        isWalking = false;
        animator.SetBool("isWalking", false);
        //animator.SetTrigger("isNotWalkingTrigger");
    }
    
    private void StopShooting()
    {
        //animator.Play("");
        isShooting = false;
        animator.SetBool("isShooting", false);
    }

    // Utils ////////////////////////////////////////////////

    private bool IsPlayerOnLeftSide()
    {
        return playerTransform.position.x < transform.position.x;
    }

    private Vector3 CalculateFoward()
    {
        if (IsPlayerOnLeftSide())
        {
            return new Vector3(-1f, 0f, 0f);
        }
        else
        {
            return new Vector3(1f, 0f, 0f);
        }
    }

    private Vector3 CalculateLookDirection()
    {
        switch (enemyProps.enemyScriptable.Pattern)
        {
            case ShootingPattern.Straight:
                return CalculateFoward();

            //case ShootingPattern.Spread:
            //    return Vector3.forward;

            case ShootingPattern.Random:
                if (Random.Range(0f, 100f) <= 30f) // 30% de chance de atirar em direção ao player
                {
                    return (playerTransform.position - transform.position).normalized;
                }
                else
                {
                    return CalculateFoward();
                }

            //case ShootingPattern.Tank:
            //    TODO
            //    return direction;

            default:
                return CalculateFoward();
        }
    }

    private IEnumerator CanAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyProps.canAttack = true;
    }

    private float CalculateDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, playerTransform.position); ;
    }



    // Test

    void Test()
    {
        while (true)
        {
            float t = 0f;
            float maxAngle = 45.0f;

            float angle = maxAngle * Mathf.Sin(t);
            t += 1.0f;
            float angleInRadians = angle * Mathf.Deg2Rad;


            Vector2 direction = new(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            direction.Normalize();
        }
    }

    // sketch

    // CastleBossSystem

    //[SerializeField] EnemyProperties[] enemyProps = null;// Make each child of Target a box with enemy properties
    // Update
    // if (enemyProps.Lenght == 0)
    // {
    // foreach (GameObject go in explosionGameObjects)
    // foreach (Animator a in explosionAnimators) // Hardcoded explosion
    // {
    // gameObject.enable = true;
    // animator.enable = true;
    // explosionParticleEffect.Play()
    // }
    // }


    // TurretBehavior
    //[SerializeField] BoxCollider2D boxCollider = null;
    //[SerializeField] Rigidbody2D rb = null;
    //[SerializeField] Transform gunBarrelPoint = null;
    //[SerializeField] Transform playerTransform = null;
    //[SerializeField] SpriteRenderer spriteRenderer = null;
    //[SerializeField] Animator animator = null;
    //[SerializeField] Sprite bulletSprite = null;

    // Control 
    //private readonly float randomDelayAfterShot = (Random.value + 1f) * 5f;

    // Update
    // Make look at player
    // shoot (and await interval randonly generated)

}
