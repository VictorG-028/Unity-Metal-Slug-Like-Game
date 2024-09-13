using UnityEngine;
using static PlayerProperties;

public class PickableWeapon : IPickable
{
    [SerializeField] private WeaponIndex weaponIndex = WeaponIndex.AK47;

    protected override bool OnPickUp()
    {
        playerProps.HoldOtherWeapon(weaponIndex);
        return false;
    }
}
