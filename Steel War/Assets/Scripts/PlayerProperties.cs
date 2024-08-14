using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
    [SerializeField] GameState gameState = null;
    [SerializeField] HpController hpController = null;


    private void OnValidate()
    {
        if (!gameState) { gameState = GameObject.Find("GameCore").GetComponent<GameState>(); }
        if (!hpController) { hpController = GameObject.Find("Canvas Manager").GetComponent<HpController>(); }
    }

    // Actions
    public bool canJump = true;
    public bool canMove = true;
    public bool canAttack = true;
    public bool canDropFromPlatform = false;
    public bool canPause = true;

    // Special properties
    public bool iFrame = false;
    public string usingName = "Knife";

    // Stats
    public int HP = 4;
    public int maxHP = 4;
    public int points = 0;

    // TODO: isso eh so um rascunho do sitema de municao, provavelmente vai mudar
    // Ammo
    public int pistol = -1; // -1 == infinite ammo
    public int ak = 90;

    // Take Damage Logic ////////////////////////////////////////////////
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && !iFrame)
        {
            HP -= 1;
            hpController.UpdatePlayerUI();
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

    // ??? ////////////////////////////////////////////////

    // Utils ////////////////////////////////////////////////

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
