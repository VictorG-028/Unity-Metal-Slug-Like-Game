using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerProperties;

public class PickableAmmo : IPickable
{
    [SerializeField] private int ammoGiven = 20;
    [SerializeField] private WeaponIndex[] weaponIndex = { WeaponIndex.AK47 };

    private void OnValidade()
    {
        if (weaponIndex == null || weaponIndex.Length == 0)
        {
            weaponIndex = (WeaponIndex[])System.Enum.GetValues(typeof(WeaponIndex));
        }
    }

    protected override bool OnPickUp()
    {
        foreach (WeaponIndex wi in weaponIndex)
        {
            playerProps.AddAmmoToWeaponByIndex(ammoGiven, wi);
        }
        return false;
    }
}
