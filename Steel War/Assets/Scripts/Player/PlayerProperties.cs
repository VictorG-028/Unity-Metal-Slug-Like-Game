using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class PlayerProperties : MonoBehaviour
{
    [SerializeField] GameState gameState = null;
    [SerializeField] HpController hpController = null;
    [SerializeField] Animator playerAnimator = null;
    [SerializeField] Collider2D playerCollider = null;
    [SerializeField] SpriteRenderer playerSpriteRenderer = null;
    [SerializeField] TextMeshProUGUI bulletUI = null;
    [SerializeField] TextMeshProUGUI scoretext = null;
    [SerializeField] Image weaponImageDisplayUI = null;
    [SerializeField] Transform weaponTransform = null;
    [SerializeField] SpriteRenderer weaponSpriteRenderer = null;
    [SerializeField] WeaponProperties holdingWeaponProps = null;
    [SerializeField] GameObject[] weapons = null; // All pickable weapons
    [SerializeField] LayerMask[] groundLayers = null;
    [SerializeField] Transform bottomCenterPoint = null;
    [SerializeField] Transform topLeftPoint = null;
    [SerializeField] Transform topRightPoint = null;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent = null;

    // Private Constants
    private readonly float minDelayBetweenShots = 0.5f;
    private readonly float maxDelayBetweenShots = 1.5f;

    private void OnValidate()
    {
        GameObject gameStateGO = GameObject.Find("GameState");
        GameObject healthBarGO = GameObject.Find("HealthBar");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject bulletCanvaGO = GameObject.Find("Bullet Text TMP");
        GameObject scoreCanvaGO = GameObject.Find("Score Text TMP");
        GameObject weaponCanvaGO = GameObject.Find("Weapon Image Display");
        GameObject weaponGO = GameObject.Find("Weapon");
        GameObject bareHandGO = GameObject.Find("BareHand");
        GameObject playerBottonCenter = GameObject.Find("Bottom Center Point");
        GameObject playerTopLeft = GameObject.Find("Top Left Point");
        GameObject playerTopRight = GameObject.Find("Top Right Point");

        if (!gameState && gameStateGO) { gameState = gameStateGO.GetComponent<GameState>(); }
        if (!hpController && healthBarGO) { hpController = healthBarGO.GetComponent<HpController>(); }
        if (!playerAnimator && player) { playerAnimator = player.GetComponent<Animator>(); }
        if (!playerCollider && player) { playerCollider = player.GetComponent<Collider2D>(); }
        if (!playerSpriteRenderer && player) { playerSpriteRenderer = player.GetComponent<SpriteRenderer>(); }
        if (!bulletUI && bulletCanvaGO) { bulletUI = bulletCanvaGO.GetComponent<TextMeshProUGUI>(); }
        if (!scoretext && scoreCanvaGO) { scoretext = scoreCanvaGO.GetComponent<TextMeshProUGUI>(); }
        if (!weaponImageDisplayUI && weaponCanvaGO) { weaponImageDisplayUI = weaponCanvaGO.GetComponent<Image>(); }
        if (!weaponTransform && weaponGO) { weaponTransform = weaponGO.GetComponent<Transform>(); }
        if (!weaponSpriteRenderer && weaponGO) { weaponSpriteRenderer = weaponGO.GetComponent<SpriteRenderer>(); }
        if (!holdingWeaponProps && bareHandGO) { holdingWeaponProps = bareHandGO.GetComponent<WeaponProperties>(); }
        weapons ??= GameObject.FindGameObjectsWithTag("Holdable Weapon");
        if (groundLayers == null || groundLayers.Length == 0)
        {
            groundLayers = new LayerMask[3];
            groundLayers[0] = LayerMask.GetMask("OneWayPlatform");
            groundLayers[1] = LayerMask.GetMask("SolidGround");
            groundLayers[2] = LayerMask.GetMask("Enemy");
        }
        OnLandEvent ??= new UnityEvent();
        if (!bottomCenterPoint && playerBottonCenter) { bottomCenterPoint = playerBottonCenter.transform; }
        if (!topLeftPoint && playerTopLeft) { topLeftPoint = playerTopLeft.transform; }
        if (!topRightPoint && playerTopRight) { topRightPoint = playerTopRight.transform; }
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
    public Vector2 GetHoldingWeaponCursorSize => new(holdingWeaponProps.weaponScriptable.cursor.width, holdingWeaponProps.weaponScriptable.cursor.height);
    public Texture2D GetCursor => holdingWeaponProps.weaponScriptable.cursor;
    public Transform GetGunBarrelPoint => holdingWeaponProps.barrelPoint;
    public Transform GetGunHandlePoint => holdingWeaponProps.handlePoint;
    public LayerMask[] GetGroundLayers => groundLayers;
    public Transform GetTopLeftPoint => topLeftPoint;
    public Transform GetTopRightPoint => topRightPoint;
    public int GetGunBulletMultiplier => holdingWeaponProps.weaponScriptable.bulletMultiplier;
    public float GetGunDelayBetweenShots => Mathf.Lerp(maxDelayBetweenShots, minDelayBetweenShots, (holdingWeaponProps.weaponScriptable.fireRate - 1) / 9.0f);
    public bool isOnGround = true;
    public bool wasGrounded = false;

    // Special properties
    public bool iFrame = false;
    private WeaponIndex currentWeaponIndex = WeaponIndex.BareHand;
    public List<GameObject> oneWayPlatformsBelowList = new();
    //public string assetPath = "Weapons/{0}.asset"; // Caminho para os ScriptableObjects

    // Stats
    public int HP = 4;
    public int maxHP = 4;
    private int points = 0;

    // Action Parameters
    [Range(0.0f, 10.0f)] public float playerSpeed           = 3f;
    [Range(0.0f, 20.0f)] public float playerJumpForce       = 6.5f;
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

    // Take Damage Logic ////////////////////////////////////////////////

    public void TakeDamage(int damage)
    {
        HP -= damage;
        hpController.UpdatePlayerUI();
        Debug.Log($"O inimigo atacou o jogador! {HP}/{maxHP}");

        if (HP <= 0)
        {
            gameState.Restart();
        }

        iFrame = true;
        StartCoroutine(DisableIFrameAfterDelay(2.0f)); // Desativa iFrame após 2 segundos
    }

    private void TakeDamageEnterLogic(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && !iFrame)
        {
            TakeDamage(1);
        }
    }

    private IEnumerator DisableIFrameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        iFrame = false;
    }

    // One Way Platform Logic /////////////////////////////////
    private void OneWayPlatformEnterLogic(Collision2D other)
    {
        if (other.gameObject.CompareTag("OneWayPlatform"))
        {
            oneWayPlatformsBelowList.Add(other.gameObject);
            canDropFromPlatform = true;
        }
    }
    private void OneWayPlatformExitLogic(Collision2D other)
    {
        if (other.gameObject.CompareTag("OneWayPlatform") && oneWayPlatformsBelowList.Contains(other.gameObject))
        {
            oneWayPlatformsBelowList.Remove(other.gameObject);
            canDropFromPlatform = oneWayPlatformsBelowList.Count != 0;
        }
    }

    // Enter/Exit Colision event

    private void OnCollisionEnter2D(Collision2D other)
    {
        TakeDamageEnterLogic(other);
        OneWayPlatformEnterLogic(other);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        OneWayPlatformExitLogic(other);
    }

    // Setters ////////////////////////////////////////////////

    public void HoldOtherWeapon(WeaponIndex newIndex)
    {
        if (currentWeaponIndex == newIndex)
        {
            ReplenishAmmo();
            return;
        }

        // Update Characteristic
        if (newIndex == WeaponIndex.BareHand || newIndex == WeaponIndex.Pistol)
            { isUsingPistolOrKnife = true; }
        else 
            { isUsingPistolOrKnife = false; }
        if (newIndex != WeaponIndex.BareHand) 
            { canAttack = true; } 
        else 
            { canAttack = false; }

        // Load weapon properties
        this.holdingWeaponProps = weapons[(int)newIndex].GetComponent<WeaponProperties>();
        //this.holdingWeaponScriptableProps = holdingWeaponProps.weaponScriptable;
        currentWeaponIndex = newIndex;

        // Use propertie to change sprite
        weaponSpriteRenderer.sprite = holdingWeaponProps.weaponScriptable.sprite;
        weaponTransform = holdingWeaponProps.handlePoint;

        UpdateBulletUI();
    }

    public void SubtractAmmo(int ammount)
    {
        holdingWeaponProps.weaponScriptable.currentAmmo -= ammount;
        UpdateBulletUI();
    }

    public void SetWeaponSpriteRendererFlipY(bool newFlipY)
    {
        weaponSpriteRenderer.flipY = newFlipY;
    }

    public void ReplenishAmmo()
    {
        holdingWeaponProps.weaponScriptable.currentAmmo = holdingWeaponProps.weaponScriptable.baseMaxAmmo;
        UpdateBulletUI();
    }

    public void AddHp(int ammount)
    {
        if (HP + ammount > maxHP)
        {
            HP = maxHP;
        }
        else
        {
            HP += ammount;
        }

        hpController.UpdatePlayerUI();
    }

    public void AddAmmoToCurrentHoldingWeapon(int ammount)
    {
        if (ammount > holdingWeaponProps.weaponScriptable.baseMaxAmmo)
        {
            holdingWeaponProps.weaponScriptable.currentAmmo = holdingWeaponProps.weaponScriptable.baseMaxAmmo;
        }
        else
        {
            holdingWeaponProps.weaponScriptable.currentAmmo += ammount;
        }
        UpdateBulletUI();
    }

    public void AddAmmoToWeaponByIndex(int ammount, WeaponIndex weaponIndex)
    {
        // Can't add ammo on infinite ammo weapons
        if (weaponIndex == WeaponIndex.BareHand) { return; }
        if (weaponIndex == WeaponIndex.Pistol) { return; }

        WeaponProperties weaponProps = weapons[(int)weaponIndex].GetComponent<WeaponProperties>();

        if (ammount > weaponProps.weaponScriptable.baseMaxAmmo)
        {
            weaponProps.weaponScriptable.currentAmmo = weaponProps.weaponScriptable.baseMaxAmmo;
        }
        else
        {
            weaponProps.weaponScriptable.currentAmmo += ammount;
        }

        UpdateBulletUI();
    }

    // Animation ////////////////////////////////////////////////
    public void OnLanding()
    {
        //print("[OnLanding] isJumping false");
        playerAnimator.SetBool("isJumping", false);
        remainingJumps = maxJumps;
        isOnGround = true;
        canJump = true;
    }


    // UI ////////////////////////////////////////////////
    private void UpdateBulletUI()
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
        weaponImageDisplayUI.SetNativeSize();
        weaponImageDisplayUI.SetNativeSize();
    }
    // Utils ////////////////////////////////////////////////
    public void ReceivingPoints(int pointsreceived)
    {
        points += pointsreceived;
        scoretext.text = points.ToString() + " PONTOS";
    }

    public bool CheckIsOnGround()
    {
        // Example of code to check if is on ground -> https://github.com/Brackeys/2D-Character-Controller/blob/master/CharacterController2D.cs
        bool check = false;

        foreach (LayerMask layer in GetGroundLayers)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                bottomCenterPoint.position, 
                Vector2.down, 
                0.75f,
                layer.value
            );
            check = hit.collider != null;
            //check = playerCollider.IsTouchingLayers(layer);
            if (check)
            {
                break;
            }
        }

        //RaycastHit2D hitTestSolidGround = Physics2D.Raycast(
        //    transform.position,
        //    Vector2.down,
        //    0.75f,
        //    LayerMask.GetMask("SolidGround")
        //);
        RaycastHit2D hitTestEnemy = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            0.75f,
            LayerMask.GetMask("Enemy")
        );
        //print($"Colidiu com enemy ? {hitTestSolidGround.collider != null} {hitTestEnemy.collider != null} {check}");
        check = check || hitTestEnemy.collider != null; // Gambiarra pra consertar um bug, não sei pq a layer Enemy não funciona dentro do loop de GroundLayers

        return check;
    }

    public enum WeaponIndex
    {
        BareHand, // 0 = (int) WeaponIndex.BareHand
        Pistol,   // 1
        AK47,     // 2
        Shotgun,  // 3
    }
}
