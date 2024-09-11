using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Placa : MonoBehaviour
{
    public TextMeshProUGUI pickupText;
    public int displayDuration = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Verifica se o personagem está no chão


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))  // Certifique-se de marcar o item com a Tag "Item"
        {

            ShowPickupText("Pressione a tecla 'Q' para ");
        }
   

    }

    private IEnumerator HideTextAfterDelay()
    {

        yield return new WaitForSeconds(displayDuration);
        pickupText.gameObject.SetActive(false);
    }

    public void ShowPickupText(string message)
    {
        pickupText.text = message;
        pickupText.gameObject.SetActive(true);
        StartCoroutine(HideTextAfterDelay());

    }
}
