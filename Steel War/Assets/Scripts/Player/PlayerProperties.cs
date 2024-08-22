using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerProperties : MonoBehaviour
{
    [SerializeField] GameState gameState = null;
    //[SerializeField] HpController hpController = null;
    [SerializeField] TextMeshProUGUI bulletUI = null;
    [SerializeField] Image weaponImageDisplayUI = null;


    private void OnValidate()
    {
        if (!gameState) { gameState = GameObject.Find("GameState").GetComponent<GameState>(); }
        //if (!hpController) { hpController = GameObject.Find("Canvas").GetComponent<HpController>(); }
        if (!bulletUI) { bulletUI = GameObject.Find("Bullet Text TMP").GetComponent<TextMeshProUGUI>(); }
        if (!weaponImageDisplayUI) { weaponImageDisplayUI = GameObject.Find("Weapon Image Display").GetComponent<Image>(); }
        
    }

    // Actions
    public bool canJump = true;
    public bool canMove = true;
    public bool canAttack = false;
    public bool canDropFromPlatform = false;
    public bool canPause = true;

    // Characteristics
    public bool isUsingPistolOrKnife = false;

    // Special properties
    public bool iFrame = false;
    public WeaponScriptable weaponProps = null;
    public GameObject holdingWeapon = null;
    public GameObject[] weapons = new GameObject[3]; // All pickable weapons
    public int currentWeaponIndex = 0;
    //public string assetPath = "Weapons/{0}.asset"; // Caminho para os ScriptableObjects

    // Stats
    public int HP = 4;
    public int maxHP = 4;
    public int points = 0;

    // Action Parameters
    [Range(1.0f, 5.0f)]
    public float playerSpeed = 1f;
    [Range(1.0f, 20.0f)]
    public float playerJumpForce = 12f;
    [Range(1, 2)]
    public int maxJumps = 1;
    [Range(0.0f, 5.0f)]
    public float melleAttackDistance = 5.0f;
    [Range(0.0f, 5.0f)]
    public float melleAttackDuration = 1.51f;
    [Range(0.0f, 5.0f)]
    public float startBulletDistance = 1.0f;
    [Range(3.0f, 100.0f)]
    public float bulletAttackDuration = 85.0f;
    [Range(0.0f, 10.0f)]
    public float bulletVelocity = 10.0f;

    // Control action logic
    public bool shouldSubtractAmmo = false;
    public int remainingJumps;

    void Start()
    {
        remainingJumps = maxJumps;
        HoldOtherWeapon(0);
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
        this.weaponProps = weapons[newIndex].GetComponent<WeaponProperties>().weaponScriptable;
        weapons[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = newIndex;
        weapons[newIndex].SetActive(true);
        
        //GameObject weaponInstance = Instantiate(newW.weaponPrefab);
        //weaponInstance.transform.SetParent(this.transform);

        canAttack = true; // TODO remover essa linha na versão Beta

        UpdateBulletUI();
    }

    // Utils ////////////////////////////////////////////////

    public void UpdateBulletUI()
    {
        if (weaponProps.baseMaxAmmo == -1)
        {
            bulletUI.text = "INFINITE AMMO";
        }
        else
        {
            bulletUI.text = string.Format("{0} / {1}", weaponProps.currentAmmo, weaponProps.baseMaxAmmo);
        }

        weaponImageDisplayUI.sprite = weapons[currentWeaponIndex].GetComponent<SpriteRenderer>().sprite;
    }
}
