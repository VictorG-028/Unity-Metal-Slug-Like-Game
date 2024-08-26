using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class PlayerProperties : MonoBehaviour
{
    [SerializeField] GameState gameState = null;
    //[SerializeField] HpController hpController = null;
    [SerializeField] Animator playerAnimator = null;
    [SerializeField] Collider2D playerCollider = null;
    [SerializeField] SpriteRenderer playerSpriteRenderer = null;
    [SerializeField] TextMeshProUGUI bulletUI = null;
    [SerializeField] Image weaponImageDisplayUI = null;
    [SerializeField] Transform weaponTransform = null;
    [SerializeField] SpriteRenderer weaponSpriteRenderer = null;
    [SerializeField] WeaponProperties holdingWeaponProps = null;
    [SerializeField] GameObject[] weapons = null; // All pickable weapons
    [SerializeField] LayerMask[] groundLayers = null;
    [SerializeField] Transform bottomCenterPoint = null;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent = null;

    private void OnValidate()
    {
        if (!gameState) { gameState = GameObject.Find("GameState").GetComponent<GameState>(); }
        //if (!hpController) { hpController = GameObject.Find("Canvas").GetComponent<HpController>(); }
        if (!playerAnimator) { playerAnimator = GameObject.Find("Player").GetComponent<Animator>(); }
        if (!playerCollider) { playerCollider = GameObject.Find("Player").GetComponent<Collider2D>(); }
        if (!playerSpriteRenderer) { playerSpriteRenderer = GameObject.Find("Player").GetComponent<SpriteRenderer>(); }
        if (!bulletUI) { bulletUI = GameObject.Find("Bullet Text TMP").GetComponent<TextMeshProUGUI>(); }
        if (!weaponImageDisplayUI) { weaponImageDisplayUI = GameObject.Find("Weapon Image Display").GetComponent<Image>(); }
        if (!weaponTransform) { weaponTransform = GameObject.Find("Weapon").GetComponent<Transform>(); }
        if (!weaponSpriteRenderer) { weaponSpriteRenderer = GameObject.Find("Weapon").GetComponent<SpriteRenderer>(); }
        if (!holdingWeaponProps) { holdingWeaponProps = GameObject.Find("BareHand").GetComponent<WeaponProperties>(); }
        weapons ??= GameObject.FindGameObjectsWithTag("Holdable Weapon");
        if (groundLayers == null)
        {
            groundLayers = new LayerMask[2]; // TODO (baixa relevância) trocar 2 por lenght
            groundLayers[0] = LayerMask.GetMask("OneWayPlatform");
            groundLayers[1] = LayerMask.GetMask("SolidGround");
        }
        if (OnLandEvent == null) { OnLandEvent = new UnityEvent(); }
        if (!bottomCenterPoint) { bottomCenterPoint = GameObject.Find("Player Bottom Center Point").transform; }
    }

    // Actions
    public bool canJump = true;
    public bool canMove = true;
    public bool canAttack = false;
    public bool canDropFromPlatform = false;
    public bool canPause = true;

    // Characteristics
    public bool isUsingPistolOrKnife = false;
    public bool isAimingToTheRightSide = true;
    public bool HasAmmo => holdingWeaponProps.weaponScriptable.currentAmmo > 0;
    public int GetHoldingWeaponDamage => holdingWeaponProps.weaponScriptable.damage;
    public Vector2 GetHoldingWeaponCursorSize => holdingWeaponProps.weaponScriptable.cursor.Size();
    public Texture2D GetCursor => holdingWeaponProps.weaponScriptable.cursor;
    public Transform GetGunBarrelPoint => holdingWeaponProps.barrelPoint;
    public Transform GetGunHandlePoint => holdingWeaponProps.handlePoint;
    //public bool IsOnGround => ; // TODO trazer lógica do playerActionController para esse script e retornar a caracteristica nesse lambda
    public bool isOnGround = true;
    public bool wasGrounded = false;

    // Special properties
    public bool iFrame = false;
    //public WeaponProperties holdingWeaponProps = null;
    //public WeaponScriptable holdingWeaponScriptableProps = null;
    //public GameObject holdingWeapon = null;
    private int currentWeaponIndex = 0;
    //public string assetPath = "Weapons/{0}.asset"; // Caminho para os ScriptableObjects

    // Stats
    public int HP = 4;
    public int maxHP = 4;
    public int points = 0;

    // Action Parameters
    //[Range(5.0f, 10.0f)] public float playerSpeed            = 3f;
   public float playerSpeed = 3f;
    //[Range(7.0f, 20.0f)] public float playerJumpForce       = 6.0f;
   public float playerJumpForce = 7.0f;
    [Range(1, 2)] public int maxJumps                       = 1;
    [Range(0.0f, 5.0f)] public float melleAttackDistance    = 5.0f;
    [Range(0.0f, 5.0f)] public float melleAttackDuration    = 1.51f;
    [Range(0.0f, 5.0f)] public float startBulletDistance    = 1.0f;
    [Range(3.0f, 100.0f)] public float bulletAttackDuration = 85.0f;
    [Range(0.0f, 10.0f)] public float bulletVelocity        = 10.0f;

    // Control action logic
    public bool shouldSubtractAmmo = false;
    public int remainingJumps;

    void Start()
    {
        remainingJumps = maxJumps;
        HoldOtherWeapon(0);
    }

    private void FixedUpdate()
    {
        // Example of code to check if is on ground -> https://github.com/Brackeys/2D-Character-Controller/blob/master/CharacterController2D.cs
        wasGrounded = isOnGround;
        isOnGround = false;
        for (int i = 0; i < groundLayers.Length; i++)
        {
            isOnGround = playerCollider.IsTouchingLayers(groundLayers[i]);
            if (isOnGround && !wasGrounded)
            {
                //OnLandEvent.Invoke();
                OnLanding();
            }
            if (isOnGround)
            {
                break;
            }
        }

        //Vector3 scanPoint = new Vector3(
        //    transform.position.x,
        //    transform.position.y - (playerSpriteRenderer.bounds.size.y / 2),
        //    transform.position.z
        //);

        //print($"{isOnGround} {wasGrounded}");

        //wasGrounded = isOnGround;
        //isOnGround = false;

        //for (int i = 0; i < groundLayers.Length; i++)
        //{
        //    Collider2D[] colliders = Physics2D.OverlapCircleAll(bottomCenterPoint.position, 4.8f, groundLayers[1]);
        //    print(colliders.Length);

        //    for (int j = 0; j < colliders.Length; j++)
        //    {
        //        if (colliders[j].gameObject != gameObject)
        //        {
        //            isOnGround = true;
        //            if (!wasGrounded)
        //                OnLandEvent.Invoke();

        //        }
        //    }
        //}
    }

    // Take Damage Logic ////////////////////////////////////////////////
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && !iFrame)
        {
            HP -= 1;
            //hpController.UpdatePlayerUI(); // TODO
            Debug.Log($"O inimigo atacou o jogador! {HP}/{maxHP}");
            
            if (HP == 0)
            {
                gameState.Restart();
            }

            iFrame = true;
            StartCoroutine(DisableIFrameAfterDelay(2.0f)); // Desativa iFrame após 2 segundos
        }
    }

    private IEnumerator DisableIFrameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        iFrame = false;
    }

    // Setters ////////////////////////////////////////////////

    public void HoldOtherWeapon(int newIndex)
    {
        // Load weapon properties
        this.holdingWeaponProps = weapons[newIndex].GetComponent<WeaponProperties>();
        //this.holdingWeaponScriptableProps = holdingWeaponProps.weaponScriptable;
        currentWeaponIndex = newIndex;

        // Use propertie to change sprite
        weaponSpriteRenderer.sprite = holdingWeaponProps.weaponScriptable.sprite;
        weaponTransform = holdingWeaponProps.handlePoint;

        canAttack = true; // TODO remover essa linha na versão Beta

        UpdateBulletUI();
    }

    public void SubtractAmmo(int ammount)
    {
        holdingWeaponProps.weaponScriptable.currentAmmo -= ammount;
    }

    public void SetWeaponSpriteRendererFlipY(bool newFlipY)
    {
        weaponSpriteRenderer.flipY = newFlipY;
    }

    // Animation ////////////////////////////////////////////////
    public void OnLanding()
    {
        //print("[OnLanding] isJumping false");
        playerAnimator.SetBool("isJumping", false);
    }


    // UI ////////////////////////////////////////////////

    public void UpdateBulletUI()
    {
        if (holdingWeaponProps.weaponScriptable.baseMaxAmmo == -1)
        {
            bulletUI.text = "INFINITE AMMO";
        }
        else
        {
            bulletUI.text = string.Format("{0} / {1}", 
                holdingWeaponProps.weaponScriptable.currentAmmo, 
                holdingWeaponProps.weaponScriptable.baseMaxAmmo
            );
        }

        weaponImageDisplayUI.sprite = holdingWeaponProps.weaponScriptable.sprite;
    }

    // Utils ////////////////////////////////////////////////
}
