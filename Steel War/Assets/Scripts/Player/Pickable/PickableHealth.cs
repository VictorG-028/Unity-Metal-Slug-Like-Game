using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableHealth : IPickable
{
    [SerializeField] int hpGiven = 1;
    protected override void OnPickUp()
    {
        playerProps.AddHp(hpGiven);
    }
}
