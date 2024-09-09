using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PlayerProperties;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField] Transform playerTransform          = null;
    [SerializeField] Rigidbody2D playerRigidyBody       = null;
    [SerializeField] Animator playerAnimator            = null;
    [SerializeField] PlayerProperties playerProps       = null;
    [SerializeField] ParticleSystem gunShotMuzzleEffect = null;
    [SerializeField] GameObject gunShotGO               = null;
    [SerializeField] Sprite bulletSprite                = null;
    [SerializeField] Transform armTransform             = null;
    [SerializeField] Vector3[] armCachedPositionsRun    = null;
    [SerializeField] Vector3[] armCachedPositionsJump   = null;
    [SerializeField] bool[] armCachedIsCalculatedRun    = null;
    [SerializeField] bool[] armCacheIsCalculatedJump    = null;

    private float horizontalSpeed = 0f;
    private Vector3 armInitialLocalPosition = Vector3.zero;
    private Vector3[] armRunInterpolatedPositions = new Vector3[7];
    private Vector3[] armJumpInterpolatedPositions = new Vector3[7];

    void OnValidate()
    {
        GameObject player = GameObject.Find("Player");
        GameObject playerArm = GameObject.Find("Arm");
        GameObject muzzleEffect = GameObject.Find("Gun Shot Muzzle Effect");

        if (!playerTransform) { playerTransform = player.GetComponent<Transform>(); }
        if (!playerRigidyBody) { playerRigidyBody = player.GetComponent<Rigidbody2D>(); }
        if (!playerAnimator) { playerAnimator = player.GetComponent<Animator>(); }
        if (!playerProps) { playerProps = player.GetComponent<PlayerProperties>(); }
        if (!gunShotMuzzleEffect) { gunShotMuzzleEffect = muzzleEffect.GetComponent<ParticleSystem>(); }
        if (!gunShotGO) { gunShotGO = GameObject.Find("Gun Point"); }
        if (!bulletSprite) { bulletSprite = Resources.Load<Sprite>("Assets/Sprites/bullet_sprite.png"); }
        if (!armTransform && playerArm) { armTransform = playerArm.transform; }
        if (armCachedPositionsRun == null) { armCachedPositionsRun = new Vector3[7]; } // TODO (baixa relevância) trocar 7 por lenght do array de sprites da animação de andar
        if (armCachedPositionsJump == null) { armCachedPositionsJump = new Vector3[7]; } // TODO (baixa relevância) trocar 7 por lenght do array de sprites da animação de pular
        if (armCachedIsCalculatedRun == null) { armCachedIsCalculatedRun = new bool[7]; } // TODO (baixa relevância) trocar 7 por lenght do array de sprites da animação de pular
        if (armCacheIsCalculatedJump == null) { armCacheIsCalculatedJump = new bool[7]; } // TODO (baixa relevância) trocar 7 por lenght do array de sprites da animação de pular

    }

    private void Start()
    {
        armInitialLocalPosition = armTransform.localPosition;
        Vector3 armFinalRunPosition = new Vector3(0.10f, 0.829f, -2f); // hardcoded end position
        Vector3 armFinalJumpPosition = new Vector3(-0.295f, 1.08f, -2f); // hardcoded end position

        //float accelerationFactor = 5.5f;
        float decelerationFactor = 0.5f;
        armRunInterpolatedPositions[0] = armInitialLocalPosition;
        armJumpInterpolatedPositions[0] = armInitialLocalPosition;
        for (int i = 1; i <= 6; i++)
        {
            // Calcula o t, que é o parâmetro de interpolação de 0 a 1
            float t = i / 6f; // 6 é o divisor pois temos 6 intervalos entre 7 pontos
            //float fasterEnd_t = Mathf.Pow(t, accelerationFactor);
            float fasterStart_t = Mathf.Pow(t, decelerationFactor);
            armRunInterpolatedPositions[i] = Vector3.Lerp(armInitialLocalPosition, armFinalRunPosition, fasterStart_t);
            armJumpInterpolatedPositions[i] = Vector3.Lerp(armInitialLocalPosition, armFinalJumpPosition, fasterStart_t);
        }
    }

    void Update()
    {
        horizontalSpeed = Input.GetAxis("Horizontal");
        playerAnimator.SetFloat("horizontalSpeed", Mathf.Abs(horizontalSpeed));

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (playerProps.canMove)
            {
                playerAnimator.SetBool("isRunning", true);

                Vector3 movement = new(horizontalSpeed, 0f, 0f);
                transform.position +=  Time.deltaTime * playerProps.playerSpeed * movement;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (playerProps.canDropFromPlatform)
            {
                playerAnimator.SetBool("isJumping", false);
                playerAnimator.SetBool("isRunning", false);

                StartCoroutine(DisableCollision());
            }
        }
        else if (Input.GetButtonDown("Jump") && Input.GetKeyDown(KeyCode.W))
        {
            if (playerProps.remainingJumps > 0 && playerProps.canJump)
            {
                playerAnimator.SetBool("isRunning", false);
                playerAnimator.SetBool("isJumping", true);

                playerRigidyBody.AddForce(new Vector2(0f, playerProps.playerJumpForce), ForceMode2D.Impulse);
                playerProps.remainingJumps--; // Decrementa o número de pulos restantes
                playerProps.isOnGround = false;
                playerProps.canJump = false;

                //StartCoroutine(ToggleAbovePlatformsCollision());
                StartCoroutine(DelayedCheckOnGround(0.2f));
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (playerProps.canAttack && (playerProps.HasAmmo || playerProps.isUsingPistolOrKnife))
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
            playerProps.HoldOtherWeapon(WeaponIndex.BareHand);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            playerProps.HoldOtherWeapon(WeaponIndex.Pistol);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            playerProps.HoldOtherWeapon(WeaponIndex.AK47);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            playerProps.HoldOtherWeapon(WeaponIndex.Shotgun);
        }
    }


    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.contacts[0].normal.y > 0.5f) // Verifica se colidiu com o chão
    //    {
    //        playerProps.remainingJumps = playerProps.maxJumps; // Reseta o número de pulos ao tocar o chão
    //    }
    //}

    private void MelleAttack()
    {
        playerProps.canAttack = false;
        playerAnimator.SetBool("isUsingKnife", true);
        armTransform.gameObject.SetActive(false);

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
        attackBehaviour.damage = playerProps.GetHoldingWeaponDamage;

        StartCoroutine(CanAttackAfterDelay(playerProps.melleAttackDuration));
        Destroy(colliderObject, playerProps.melleAttackDuration); // Destroi o Collider ap�s o tempo de ataque
    }

    private void ShootBullet(bool shouldSubtractAmmo)
    {
        playerProps.canAttack = false;


        if (shouldSubtractAmmo) playerProps.SubtractAmmo(1);

        Vector3 lookDirection = CalculateLookDirection();

        gunShotMuzzleEffect.transform.position = playerProps.GetGunBarrelPoint.position - 0.2f * lookDirection; // Gambiarra pra coocar na posição do cano da arma
        gunShotMuzzleEffect.Play();

        GameObject bulletObject = new("Bullet") { tag = "Attack" };
        //bulletObject.transform.position = transform.position + lookDirection * playerProps.startBulletDistance;
        bulletObject.transform.position = playerProps.GetGunBarrelPoint.position;

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
        attackBehaviour.damage = playerProps.GetHoldingWeaponDamage;

        StartCoroutine(CanAttackAfterDelay(0.5f));
        //StartCoroutine(MoveBullet(bulletObject));
        Destroy(bulletObject, playerProps.bulletAttackDuration);
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

    //private IEnumerator MoveBullet(GameObject movingObject)
    //{
    //    float timer = 0f;
    //    while (timer < playerProps.bulletAttackDuration)
    //    {
    //        if (movingObject == null)
    //            yield break; // Sai da coroutine se o objeto Bullet for destruído prematuramente

    //        movingObject.transform.position += playerProps.bulletVelocity * Time.deltaTime * movingObject.transform.forward;
    //        timer += Time.deltaTime;
    //        yield return null; // Aguarda um frame
    //    }
    //    Destroy(movingObject);
    //}

    private IEnumerator DelayedCheckOnGround(float delay)
    {
        yield return new WaitForSeconds(delay);

        bool onGround;
        while (true)
        {
            onGround = playerProps.CheckIsOnGround();

            if (onGround) // Se o jogador estiver no chão, saímos do loop
            {
                playerProps.OnLanding();
                yield break; // Encerra a coroutine
            }

            // Aguardamos até o próximo frame para continuar a verificar
            yield return null;
        }
    }

    //private IEnumerator ToggleAbovePlatformsCollision()
    //{
    //    IEnumerable<RaycastHit2D> platforms = GetAboveOneWayPlatforms();

    //    List<Collider2D> platformsColliders = platforms
    //        .Where(hit => hit.transform.CompareTag("OneWayPlatform"))
    //        .Select(hit => hit.transform.GetComponent<Collider2D>())
    //        .Where(c => c != null)
    //        .ToList();
    //    print($"{platformsColliders.Count} {platformsColliders}");

    //    platformsColliders.ForEach(collider => collider.enabled = false);

    //    IEnumerable<RaycastHit2D> afterPlatforms = platforms;

    //    while (true)
    //    {
    //        if (afterPlatforms.Count() < platforms.Count())
    //        {
    //            platformsColliders.ForEach(collider => collider.enabled = true);
    //            break;
    //        }
    //        afterPlatforms = GetAboveOneWayPlatforms();
    //        yield return null;
    //    }
    
    //}

    private IEnumerator CanAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerProps.canAttack = true;
        playerAnimator.SetBool("isUsingKnife", false);
        armTransform.gameObject.SetActive(true);
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D[] platformColliders = playerProps.oneWayPlatformsBelowList
                                                       .Select(p => p.GetComponent<BoxCollider2D>())
                                                       .ToArray();
        BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();
        float disableTime = 0.5f;

        // Desativa a colisão para permitir que o personagem caia
        foreach (BoxCollider2D platformCollider in platformColliders)
        {
            //platformCollider.enabled = false;
            Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
        }

        yield return new WaitForSeconds(disableTime);

        // Reativa a colisão após o tempo especificado
        foreach (BoxCollider2D platformCollider in platformColliders)
        {
            //platformCollider.enabled = true;
            Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        }
    }

    public void RepositionArmRunAnimationEvent(int t)
    {
        armTransform.localPosition = armRunInterpolatedPositions[t];

        //if (armCachedIsCalculatedRun[t] == false)
        //{
        //    armCachedPositionsRun[t] = armTransform.localPosition;
        //    armCachedIsCalculatedRun[t] = true;

        //    // Move arm to be consistent with animation
        //    armTransform.localPosition = new Vector3(
        //        armTransform.localPosition.x + 0.0010f * t * t,
        //        armTransform.localPosition.y - 0.0014f * t * t,
        //        armTransform.localPosition.z
        //    );
        //}
        //else
        //{
        //    armTransform.localPosition = armCachedPositionsRun[t];
        //}
    }

    public void RepositionArmJumpAnimationEvent(int t)
    {
        armTransform.localPosition = armJumpInterpolatedPositions[t];

        //if (armCacheIsCalculatedJump[t] == false)
        //{
        //    armCachedPositionsJump[t] = armTransform.localPosition;
        //    armCacheIsCalculatedJump[t] = true;

        //    // Move arm to be consistent with animation
        //    armTransform.localPosition = new Vector3(
        //        armTransform.localPosition.x - 0.0100f * t - 0.0002f * t * t * t,
        //        armTransform.localPosition.y + 0.0200f * t - 0.0002f * t * t * t,
        //        armTransform.localPosition.z
        //    );
        //}
        //else
        //{
        //    armTransform.localPosition = armCachedPositionsJump[t];
        //}
    }

    public void ResetArmPosition()
    {
        armTransform.localPosition = armInitialLocalPosition;
    }

    //private IEnumerable<RaycastHit2D> GetAboveOneWayPlatforms()
    //{
    //    RaycastHit2D[] leftHits = Physics2D.RaycastAll(
    //        playerProps.GetTopLeftPoint.position,
    //        Vector2.up,
    //        15.0f,
    //        LayerMask.GetMask("OneWayPlatform")
    //    );
    //    RaycastHit2D[] rightHits = Physics2D.RaycastAll(
    //        playerProps.GetTopRightPoint.position,
    //        Vector2.up,
    //        15.0f,
    //        LayerMask.GetMask("OneWayPlatform")
    //    );

    //    return leftHits.Union(rightHits);
    //}
}
