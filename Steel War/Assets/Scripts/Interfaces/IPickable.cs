using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPickable : MonoBehaviour
{
    protected PlayerProperties playerProps = null;
    protected AudioSource pickUpSound = null;
    protected SpriteRenderer spriteRenderer = null;

    // Control
    private bool hasBeenPicked = false;
    private bool hasSound = false;

    private void OnValidate()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (!playerProps && player != null) { playerProps = player.GetComponent<PlayerProperties>(); }
        //if (!pickUpSound) { pickUpSound = GetComponent<AudioSource>(); } // Can't Get component on Interface
        //if (!spriteRenderer) { spriteRenderer = GetComponent<SpriteRenderer>(); }
    }

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (!playerProps && player != null) { playerProps = player.GetComponent<PlayerProperties>(); }
    }

    protected abstract bool OnPickUp();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && !hasBeenPicked)
        {
            hasBeenPicked = true;
            hasSound = OnPickUp();
            if(hasSound)
            {
                Destroy(gameObject, 2.0f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}