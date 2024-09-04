using System;
using System.Collections;
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
    //[SerializeField] ShootingPolicy shootingPolicy = ShootingPolicy.OnlyForward;


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
    }

    [Range(0.0f, 10.0f)] public float delayBetweenAttacks = 6.0f;
    [Range(0.0f, 15.0f)] public float minCloseDistance = 10.5f;
    [Range(1.0f, 15.0f)] public float bulletVelocidy = 10.0f;
    [Range(3.0f, 100.0f)] public float bulletDuration = 10.0f;
    [Range(0, 10)] public int bulletDamage = 1;

    void Update()
    {
        if (enemyProps.canAttack)
        {
            print($"Inimigo {this.name} deve atirar");
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        enemyProps.canAttack = false;

        Vector3 lookDirection = CalculateLookDirection();

        GameObject bulletObject = new("Bullet") { tag = "Attack" };
        bulletObject.transform.position = gunBarrelPoint.position;

        // Adiciona um sprite renderer com um sprite
        SpriteRenderer spriteRenderer = bulletObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = bulletSprite;
        bulletObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Adiciona um colider e rigidyBody
        BoxCollider2D collider = bulletObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1.0f, 1.0f); // Largura, Altura
        collider.isTrigger = true;
        Rigidbody2D rigidbody = bulletObject.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        rigidbody.velocity = lookDirection * bulletVelocidy;
        rigidbody.gravityScale = 0;

        // Adiciona lógica de colisão e dano
        AttackBehaviour attackBehaviour = bulletObject.AddComponent<AttackBehaviour>();
        attackBehaviour.damage = bulletDamage;
        attackBehaviour.isShootedByEnemy = true;

        StartCoroutine(CanAttackAfterDelay(delayBetweenAttacks));
        Destroy(bulletObject, bulletDuration);
    }

    // Enum ////////////////////////////////////////////////
    //private enum ShootingPolicy
    //{
    //    OnlyForward,
    //    RandomlyBetweenForwardAndTarget,
    //    AlternateDirections
    //}

    // Utils ////////////////////////////////////////////////

    private Vector3 CalculateFoward()
    {
        if (playerTransform.position.x < transform.position.x)
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
        //animator.SetBool("isShooting", false); // TODO: adicionar as animações do Basic Soldier
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
