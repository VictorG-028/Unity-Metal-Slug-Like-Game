using UnityEngine;
public class WeaponProperties : MonoBehaviour
{
    public string weaponName                  = "NewWeapon"; // Must be same name as fileName to auto load
    public int damage                         = 1;
    public int baseMaxAmmo                    = 0;
    public int currentAmmo                    = 0;
    public Sprite weaponIcon                  = null;
    //public Sprite aim                       = null; // TODO trazer prop aim do script aim para esse arquivo
    public WeaponScriptable weaponScriptable = null;

    private void OnValidate()
    {
        //if (!weaponIcon) { weaponIcon = } // TODO: Load any "broken" weapon sprite here
        //if (!weaponScriptable) { weaponScriptable = GameObject.Find("TODO"); }
    }
}