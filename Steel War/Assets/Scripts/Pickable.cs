using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField] PlayerProperties playerProps = null;


    private void OnValidate()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (!playerProps && player != null) { playerProps = player.GetComponent<PlayerProperties>(); }
    }

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (!playerProps && player != null) { playerProps = player.GetComponent<PlayerProperties>(); }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            if (gameObject.CompareTag("Holdable Weapon"))
            {
                playerProps.HoldOtherWeapon(2); // TODO - Criar um campo nessa classe para definir qual arma vai segurar
                Destroy(gameObject);
            } 
            else if (gameObject.CompareTag("Points"))
            {
                playerProps.ReceivingPoints(50); // TODO - criar um campo que define quantos pontos esse item acrescenta
                Destroy(gameObject);
            }
        }
    }
}
