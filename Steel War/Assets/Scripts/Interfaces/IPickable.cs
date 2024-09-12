using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPickable : MonoBehaviour
{
    protected PlayerProperties playerProps;

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

    protected abstract void OnPickUp();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            OnPickUp();
            Destroy(gameObject);
        }
    }
}