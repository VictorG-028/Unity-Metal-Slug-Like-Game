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
        if (!bulletSprite) { bulletSprite = Resources.Load<Sprite>("Assets/Sprites/Enemy_soldier_bullet_sprite.png"); }
        if (!animator) { animator = GetComponent<Animator>(); }
        if (!spriteRenderer) { spriteRenderer = GetComponent<SpriteRenderer>(); }
    }

    // Public parameters
    public bool isBrave = false;
    [Range(0.0f, 10.0f)] public float delayBetweenAttacksMin = 2.0f;
    [Range(0.0f, 10.0f)] public float delayBetweenAttacksMax = 5.0f;
    [Range(0.0f, 15.0f)] public float minCloseDistance = 8.5f;
    [Range(0.0f, 50.0f)] public float maxCloseDistance = 16.5f;
    [Range(1.0f, 15.0f)] public float bulletVelocidy = 10.0f;
    [Range(3.0f, 100.0f)] public float bulletDuration = 10.0f;
    [Range(0, 10)] public int bulletDamage = 1;
    [Range(0, 10)] public int tankBulletDamage = 2;
    [Range(1.0f, 15.0f)] public float tankBulletForce = 15.0f;
    [Range(1.0f, 15.0f)] public float arcHeightMin = 1.0f;
    [Range(1.0f, 15.0f)] public float arcHeightMax = 8.0f;
    [Range(1.0f, 10.0f)] public float walkingSpeed = 5f;

    // Control
    //private bool isBusyDoingAction = false;
    //private bool hasTriggerredAnimation = false;
    private bool isAwaitingInitialAnimationFinish = true;
    private bool isShooting = false;
    private bool isWalking = false;
    private bool isGoingToWalk = false;
    private bool canShoot = false;
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
        canShoot = distanceToPlayer < maxCloseDistance;
        info = animator.GetCurrentAnimatorStateInfo(0);
        //isShooting = info.IsName("Shooting");
        //isWalking = info.IsName("Walking") || info.IsName("Pulling In Gun");
        //isStandingStill = info.IsName("PointingGunStanding");

        //print($"Dist�ncia: {distanceToPlayer} < {minCloseDistance} -> {isGoingToWalk}");
        //print($"Lenght da anima��o: {info.length} | ");
        //print($"{enemyProps.canAttack} && {info.IsName("Pointing Gun Standing")} {enemyProps.canAttack && info.IsName("Pointing Gun Standing")}");

        if (isAwaitingInitialAnimationFinish && ! isBrave)
        {
            isAwaitingInitialAnimationFinish = !info.IsName("Pointing Gun Standing");
        }
        else if (isShooting)
        {
            StopShooting();
        }
        else if (isGoingToWalk && ! isBrave)
        {
            WalkOpositeDirection();
        }
        else if (isWalking && ! isBrave)
        {
            StopWalking();
        }
        else if (enemyProps.canAttack && (info.IsName("Pointing Gun Standing") || (enemyProps.enemyScriptable.Pattern == ShootingPattern.Tank)) && canShoot)
        {
            //print($"Inimigo {this.name} deve atirar");
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        enemyProps.canAttack = false;
        spriteRenderer.flipX = !IsPlayerOnLeftSide(); // Make sprite face the opposite side of the player
        isShooting = true;
        animator.SetBool("isShooting", true);

        lookDirection = CalculateLookDirection();

        // Check if the enemy is a tank
        if (enemyProps.enemyScriptable.Pattern == ShootingPattern.Tank)
        {
            StartCoroutine(TankShot(0.6f));
        }
        else
        {
            // Regular bullet behavior for other enemies
            bulletObject = new("Bullet") { tag = "Attack" };
            bulletObject.transform.position = gunBarrelPoint.position;

            bulletSpriteRenderer = bulletObject.AddComponent<SpriteRenderer>();
            bulletSpriteRenderer.sprite = bulletSprite;
            bulletObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            bulletCollider = bulletObject.AddComponent<BoxCollider2D>();
            bulletCollider.size = new Vector2(1.0f, 1.0f);
            bulletCollider.isTrigger = true;

            bulletRigidbody = bulletObject.AddComponent<Rigidbody2D>();
            bulletRigidbody.bodyType = RigidbodyType2D.Dynamic;
            bulletRigidbody.velocity = lookDirection * bulletVelocidy;
            bulletRigidbody.gravityScale = 0;

            bulletAttackBehaviour = bulletObject.AddComponent<AttackBehaviour>();
            bulletAttackBehaviour.damage = bulletDamage;
            bulletAttackBehaviour.isShootedByEnemy = true;

            Destroy(bulletObject, bulletDuration);
        }
        
        float randomizedDelay = UnityEngine.Random.Range(delayBetweenAttacksMin, delayBetweenAttacksMax);
        StartCoroutine(CanAttackAfterDelay(randomizedDelay));
    }

    // This method calculates the firing direction and force for the cannonball to hit the player
    private Vector2 CalculateFiringDirection(Vector2 startPosition, Vector2 targetPosition)
    {
        // You can adjust this to calculate the arc for the cannonball
        Vector2 direction = targetPosition - startPosition;
        float randomizedArcHeight = UnityEngine.Random.Range(arcHeightMin, arcHeightMax);
        direction.y += randomizedArcHeight; // Add a vertical component for the arc (adjust arcHeight as needed)
        return direction.normalized;
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
            // Dire��o oposta ao jogador no eixo x
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
                if (Random.Range(0f, 100f) <= 30f) // 30% de chance de atirar em dire��o ao player
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

    private IEnumerator TankShot(float delay)
    {   
        yield return new WaitForSeconds(delay);
        // Create a larger round bullet for the tank
        bulletObject = new("CannonBall") { tag = "Attack" };
        bulletObject.transform.position = gunBarrelPoint.position;

        bulletSpriteRenderer = bulletObject.AddComponent<SpriteRenderer>();
        bulletSpriteRenderer.sprite = bulletSprite; // Use a round sprite for the cannonball
        bulletObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // Make it bigger

        // Add collider and rigidbody with gravity for arcing effect
        bulletCollider = bulletObject.AddComponent<BoxCollider2D>();
        bulletCollider.isTrigger = true; // Collides with the ground

        bulletRigidbody = bulletObject.AddComponent<Rigidbody2D>();
        bulletRigidbody.bodyType = RigidbodyType2D.Dynamic;
        bulletRigidbody.gravityScale = 1; // Enable gravity for the arcing trajectory

        // Calculate the force to make the cannonball reach the player's location
        Vector2 targetPosition = playerTransform.position; // Assuming playerTransform is available
        Vector2 firingDirection = CalculateFiringDirection(gunBarrelPoint.position, targetPosition);
        bulletRigidbody.AddForce(firingDirection * tankBulletForce, ForceMode2D.Impulse);

        // Add explosion behavior on impact
        bulletAttackBehaviour = bulletObject.AddComponent<AttackBehaviour>();
        bulletAttackBehaviour.damage = tankBulletDamage;
        bulletAttackBehaviour.isShootedByEnemy = true;

        // Destroy the cannonball after a certain time or on impact
        Destroy(bulletObject, bulletDuration);
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
}
