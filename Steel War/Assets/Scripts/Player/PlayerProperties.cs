using System.Collections;
using System.Collections.Generic;
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
        //if (!hpController) { hpController = GameObject.Find("Canvas Manager").GetComponent<HpController>(); }
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

    public void Hold(int newIndex)
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
