using UnityEngine;
using static PlayerProperties;

public class PickableWeapon : IPickable
{
    [SerializeField] private WeaponIndex weaponIndex = WeaponIndex.AK47;

    protected override void OnPickUp()
    {
        playerProps.HoldOtherWeapon(weaponIndex);
    }
}
