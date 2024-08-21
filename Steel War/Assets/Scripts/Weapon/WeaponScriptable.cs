using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponScriptable : ScriptableObject
{
    public string weaponName       = "NewWeapon"; // Must be same name as fileName to auto load
    public int damage              = 1;
    public int baseMaxAmmo         = 0;
    public int currentAmmo         = 0;
    public Sprite weaponIcon       = null;
    //public Sprite aim            = null; // TODO trazer prop aim do script aim para esse arquivo
    public GameObject weaponPrefab = null;

    private void OnValidate()
    {
        // TODO: Load any "broken" weapon sprite here
        //if (!weaponIcon) { weaponIcon = }
        //if (!weaponPrefab) { weaponPrefab = Resources.Load("TODO"); }
    }
}