using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform = null;
    [SerializeField] Rigidbody2D playerRigidyBody      = null;
    [SerializeField] Animator playerAnimator           = null;
    [SerializeField] PlayerProperties playerProps      = null;
    [SerializeField] Camera mainCamera                 = null;
    [SerializeField] ParticleSystem gunShotEffect      = null;
    [SerializeField] GameObject gunShotGO              = null;
    [SerializeField] Sprite bulletSprite               = null;

    void OnValidate()
    {
        if (!playerTransform) { playerTransform = GameObject.Find("Player").GetOrAddComponent<Transform>(); }
        if (!playerRigidyBody) { playerRigidyBody = GameObject.Find("Player").GetComponent<Rigidbody2D>(); }
        if (!playerAnimator) { playerAnimator = GameObject.Find("Player").GetComponent<Animator>(); }
        if (!playerProps) { playerProps = GameObject.Find("Player").GetComponent<PlayerProperties>(); }
        if (!mainCamera) { mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); }
        if (!gunShotEffect) { gunShotEffect = GameObject.Find("Gun Shot Effect").GetComponent<ParticleSystem>(); }
        if (!gunShotGO) { gunShotGO = GameObject.Find("Gun Point"); }
        if (!bulletSprite) { bulletSprite = Resources.Load<Sprite>("TODO"); }
    }

    void Update()
    {
        applyMovementForce();

        if (Input.GetButtonDown("Jump"))
        {
            if (playerProps.remainingJumps > 0 && playerProps.canJump)
            { 
                playerRigidyBody.AddForce(new Vector2(0f, playerProps.playerJumpForce), ForceMode2D.Impulse);
                playerProps.remainingJumps--; // Decrementa o número de pulos restantes
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (playerProps.canAttack && (playerProps.weaponProps.currentAmmo > 0 || playerProps.isUsingPistolOrKnife))
            {
                playerProps.shouldSubtractAmmo = !playerProps.isUsingPistolOrKnife;
                print("Atirou");
                ShootBullet(playerProps.shouldSubtractAmmo);
            }

        } 
        else if (Input.GetMouseButtonDown(1))
        {
            if (playerProps.canAttack)
            {
                print("Usou faca");
                MelleAttack();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            playerProps.HoldOtherWeapon(0); // Troca para faca
            playerProps.isUsingPistolOrKnife = true;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            playerProps.HoldOtherWeapon(1); // Troca para pistola
            playerProps.isUsingPistolOrKnife = true;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            playerProps.HoldOtherWeapon(2); // Troca para AK-47
            playerProps.isUsingPistolOrKnife = false;
        }
    }

    void applyMovementForce()
    {
        if (playerProps.canMove) {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
            transform.position += movement * Time.fixedDeltaTime * playerProps.playerSpeed;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f) // Verifica se colidiu com o chão
        {
            playerProps.remainingJumps = playerProps.maxJumps; // Reseta o número de pulos ao tocar o chão
        }
    }

    private void MelleAttack()
    {
        playerProps.canAttack = false;
        // TODO: Play melle Visual Effect
        // gunShotGO must be positioned on the gun end
        //gunShotEffect.Play();

        // Create collider to check for damage
        Vector3 lookDirection = CalculateLookDirection();
        GameObject colliderObject = new("MelleCollider") { tag = "Attack" };
        colliderObject.transform.forward = lookDirection;
        colliderObject.transform.position = transform.position + lookDirection * playerProps.melleAttackDistance;
        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>(); // Adiciona um Collider
        collider.size = new Vector2(0.5f, 2.0f); // Largura, Altura
        collider.isTrigger = true;

        // Adiciona lógica de colisão e dano
        AttackBehaviour attackBehaviour = colliderObject.AddComponent<AttackBehaviour>();
        attackBehaviour.damage = playerProps.weaponProps.damage;

        StartCoroutine(canAttackAfterDelay(1.5f));
        Destroy(colliderObject, playerProps.melleAttackDuration); // Destroi o Collider ap�s o tempo de ataque
    }

    private void ShootBullet(bool shouldSubtractAmmo)
    {
        // TODO: Play gunShotEffect Visual Effect
        // gunShotGO must be positioned on the gun end
        //gunShotEffect.Play();


        playerProps.canAttack = false;
        if (shouldSubtractAmmo) playerProps.weaponProps.currentAmmo -= 1;
        playerProps.UpdateBulletUI();

        Vector3 lookDirection = CalculateLookDirection();

        GameObject bulletObject = new("Bullet") { tag = "Attack" };
        bulletObject.transform.position = transform.position + lookDirection * playerProps.startBulletDistance;

        // Adiciona um sprite renderer com um sprite
        SpriteRenderer spriteRenderer = bulletObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = bulletSprite;
        bulletObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Adiciona um colider e rigidyBody
        BoxCollider2D collider = bulletObject.AddComponent<BoxCollider2D>();
        //collider.center = new Vector3(0, 0.2f, 0); // x, y, z
        collider.size = new Vector2(1.0f, 1.0f); // Largura, Altura
        collider.isTrigger = true;
        Rigidbody2D rigidbody = bulletObject.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        rigidbody.velocity = lookDirection * playerProps.bulletVelocity;
        rigidbody.gravityScale = 0;
        //rigidbody.simulated = true;

        // Adiciona lógica de colisão e dano
        AttackBehaviour attackBehaviour = bulletObject.AddComponent<AttackBehaviour>();
        attackBehaviour.damage = playerProps.weaponProps.damage;

        StartCoroutine(canAttackAfterDelay(0.5f));
        //StartCoroutine(MoveBullet(bulletObject));
    }

    // Utils ////////////////////////////////////////////////

    private Vector3 CalculateLookDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePosition - playerTransform.position).normalized;
        direction.z = 1; // Previne bug que faz a bala andar para trás do cenário
        //print(direction);
        return direction;
    }

    private IEnumerator MoveBullet(GameObject movingObject)
    {
        float timer = 0f;
        while (timer < playerProps.bulletAttackDuration)
        {
            if (movingObject == null)
                yield break; // Sai da coroutine se o objeto Bullet for destruído prematuramente

            movingObject.transform.position += playerProps.bulletVelocity * Time.deltaTime * movingObject.transform.forward;
            timer += Time.deltaTime;
            yield return null; // Aguarda um frame
        }
        Destroy(movingObject);
    }

    private IEnumerator canAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerProps.canAttack = true;
    }
}
