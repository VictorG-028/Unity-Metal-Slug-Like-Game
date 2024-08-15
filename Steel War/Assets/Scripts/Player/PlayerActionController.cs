using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEditor;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField] private GameObject player = null;
    [SerializeField] Animator animator = null;
    [SerializeField] PlayerProperties playerProps = null;
    [SerializeField] Camera mainCamera = null;
    //[SerializeField] ParticleSystem gunShotEffect = null; // TODO: criar gameObject do efeito do tiro
    //[SerializeField] GameObject gunShotGO = null;
    [SerializeField] Sprite bulletSprite = null;


    // Parameters
    [Range(0.0f, 5.0f)]
    [SerializeField] float melleAttackDistance = 5.0f;
    [Range(0.0f, 5.0f)]
    [SerializeField] float melleAttackDuration = 1.51f;
    [Range(0.0f, 5.0f)]
    [SerializeField] float startBulletDistance = 1.0f;
    [Range(3.0f, 100.0f)]
    [SerializeField] float bulletAttackDuration = 85.0f;
    [Range(0.0f, 10.0f)]
    [SerializeField] float bulletVelocity = 10.0f;

    // Control
    private bool shouldSubtractAmmo = false;


    void OnValidate()
    {
        if (!player) { player = GameObject.Find("Player"); }
        if (!animator) { animator = gameObject.GetComponent<Animator>(); }
        if (!playerProps) { playerProps = gameObject.GetComponent<PlayerProperties>(); }
        if (!mainCamera) { mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); }
        //if (!gunShotEffect) { gunShotEffect = GameObject.Find("Gun Shot Effect").GetComponent<ParticleSystem>(); }
        //if (!gunShotGO) { gunShotGO = GameObject.Find("Sword Slash Effect"); }
        if (!bulletSprite) { bulletSprite = Resources.Load<Sprite>("TODO"); }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (playerProps.canAttack && (playerProps.weaponProps.currentAmmo > 0 || playerProps.isUsingPistolOrKnife))
            {
                shouldSubtractAmmo = !playerProps.isUsingPistolOrKnife;
                print("Atirou");
                ShootBullet(shouldSubtractAmmo);
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
            playerProps.Hold(0); // Troca para faca
            playerProps.isUsingPistolOrKnife = true;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            playerProps.Hold(1); // Troca para pistola
            playerProps.isUsingPistolOrKnife = true;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            playerProps.Hold(2); // Troca para AK-47
            playerProps.isUsingPistolOrKnife = false;
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
        colliderObject.transform.position = transform.position + lookDirection * melleAttackDistance;
        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>(); // Adiciona um Collider
        collider.size = new Vector2(0.5f, 2.0f); // Largura, Altura
        collider.isTrigger = true;

        // Adiciona lógica de colisão e dano
        AttackBehaviour attackBehaviour = colliderObject.AddComponent<AttackBehaviour>();
        attackBehaviour.damage = playerProps.weaponProps.damage;

        StartCoroutine(canAttackAfterDelay(1.5f));
        Destroy(colliderObject, melleAttackDuration); // Destroi o Collider ap�s o tempo de ataque
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
        bulletObject.transform.position = transform.position + lookDirection * startBulletDistance;

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
        rigidbody.velocity = lookDirection * bulletVelocity;
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
        Vector3 direction = (mousePosition - player.transform.position).normalized;
        direction.z = 1; // Previne bug que faz a bala andar para trás do cenário
        //print(direction);
        return direction;
    }

    private IEnumerator MoveBullet(GameObject movingObject)
    {
        float timer = 0f;
        while (timer < bulletAttackDuration)
        {
            if (movingObject == null)
                yield break; // Sai da coroutine se o objeto Bullet for destruído prematuramente

            movingObject.transform.position += bulletVelocity * Time.deltaTime * movingObject.transform.forward;
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
