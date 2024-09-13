using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableHealth : IPickable
{
    [SerializeField] int hpGiven = 1;
    protected override bool OnPickUp()
    {
        playerProps.AddHp(hpGiven);
        pickUpSound = GetComponent<AudioSource>();
        if (pickUpSound != null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            pickUpSound.Play();
            spriteRenderer.enabled = false;
            return true;
        }
        return false;
    }
}
