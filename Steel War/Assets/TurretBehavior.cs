using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

public class TurretBehavior : MonoBehaviour
{
    [SerializeField] EnemyProperties enemyProps = null;
    [SerializeField] BoxCollider2D boxCollider = null;
    [SerializeField] Rigidbody2D rb = null;
    [SerializeField] Transform gunBarrelPoint = null;
    [SerializeField] Transform playerTransform = null;
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Animator animator = null;
    [SerializeField] Sprite bulletSprite = null;
    [SerializeField] ParticleSystem gunShotMuzzleEffect = null;


    void OnValidate()
    {
        if (!enemyProps) { enemyProps = GetComponent<EnemyProperties>(); }
        if (!boxCollider) { boxCollider = GetComponent<BoxCollider2D>(); }
        if (!rb) { rb = GetComponent<Rigidbody2D>(); }
        if (!gunBarrelPoint) { gunBarrelPoint = GetComponent<Transform>(); }
        if (!playerTransform) { playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); }
        if (!spriteRenderer) { spriteRenderer = GetComponent<SpriteRenderer>(); }
        if (!animator) { animator = GetComponent<Animator>(); }
        if (!bulletSprite) { bulletSprite = Resources.Load<Sprite>("Assets/Sprites/Enemy_soldier_bullet_sprite.png"); }
        if (!gunShotMuzzleEffect) { gunShotMuzzleEffect = GetComponent<ParticleSystem>(); }
    }

    // Public parameters
    [Range(1.0f, 15.0f)] public float bulletVelocidy = 10.0f;
    [Range(3.0f, 100.0f)] public float bulletDuration = 10.0f;
    [Range(0, 10)] public int bulletDamage = 1;

    // Control 
    private float randomDelayAfterShot = 5f;
    private Vector3 direction = Vector3.zero;
    private float angle = 0f;
    private AnimatorStateInfo info;
    private bool isAwaitingInitialAnimationFinish = true;
    private GameObject bulletObject = null;
    private SpriteRenderer bulletSpriteRenderer = null;
    private BoxCollider2D bulletCollider = null;
    private Rigidbody2D bulletRigidbody = null;
    private AttackBehaviour bulletAttackBehaviour = null;

    private void Start()
    {
        randomDelayAfterShot = (Random.value + 1f) * 4.0f;
    }

    void Update()
    {
        direction = CalculateLookDirection();
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Ângulo entre Vector2D(1,0) e direction
        info = animator.GetCurrentAnimatorStateInfo(0);

        LookAtPlayer(angle);
        
        if (isAwaitingInitialAnimationFinish)
        {
            isAwaitingInitialAnimationFinish = !info.IsName("Default");
        }
        else if (enemyProps.canAttack && info.IsName("Default"))
        {
            ShootBullet(-direction);
        }
    }

    private void ShootBullet(Vector3 directionToShoot)
    {
        enemyProps.canAttack = false;

        // Muzzle particle effect
        gunShotMuzzleEffect.transform.position = gunBarrelPoint.position;
        angle = Mathf.Atan2(directionToShoot.y, directionToShoot.x);
        gunShotMuzzleEffect.startRotation3D = new Vector3(0, 0, -angle +Mathf.PI/2);
        gunShotMuzzleEffect.Play();

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
        bulletRigidbody.velocity = directionToShoot * bulletVelocidy;
        bulletRigidbody.gravityScale = 0;

        // Adiciona lógica de colisão e dano
        bulletAttackBehaviour = bulletObject.AddComponent<AttackBehaviour>();
        bulletAttackBehaviour.damage = bulletDamage;
        bulletAttackBehaviour.isShootedByEnemy = true;

        StartCoroutine(CanAttackAfterDelay(randomDelayAfterShot));
        Destroy(bulletObject, bulletDuration);
    }

    // Util ////////////////////////////////////////////////////////////

    private void LookAtPlayer(float angle)
    {
        transform.localRotation = Quaternion.Euler(
            0f,
            0f,
            angle
        );
    }

    private IEnumerator CanAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyProps.canAttack = true;
    }

    private Vector3 CalculateLookDirection()
    {
        return (transform.position - playerTransform.transform.position).normalized;
    }
}
