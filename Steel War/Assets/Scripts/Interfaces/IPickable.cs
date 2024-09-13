using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPickable : MonoBehaviour
{
    protected PlayerProperties playerProps;
    protected AudioSource audio;
    protected SpriteRenderer spriteRenderer;

    // Control
    private bool hasBeenPicked = false;

    private void OnValidate()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (!playerProps && player != null) { playerProps = player.GetComponent<PlayerProperties>(); }
        if (!audio) { audio = GetComponent<AudioSource>(); }
        if (!spriteRenderer) { spriteRenderer = GetComponent<SpriteRenderer>(); }
    }

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (!playerProps && player != null) { playerProps = player.GetComponent<PlayerProperties>(); }
    }

    protected abstract void OnPickUp();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && !hasBeenPicked)
        {
            hasBeenPicked = true;
            OnPickUp();
            if(audio)
            {
                audio.Play();
                spriteRenderer.enabled = false;
                Destroy(gameObject, 2.0f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}