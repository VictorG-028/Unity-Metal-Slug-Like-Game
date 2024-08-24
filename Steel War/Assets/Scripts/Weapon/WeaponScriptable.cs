using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponScriptable : ScriptableObject
{
    public string weaponName         = "NewWeapon"; // Must be same name as fileName to auto load
    [Range(0, 30)] public int damage = 1;
    public int baseMaxAmmo           = 0;
    public int currentAmmo           = 0;
    public Sprite sprite             = null;
    public Texture2D cursor          = null;


    private void OnValidate()
    {
        //if (!sprite) { sprite = }// TODO: Load any "broken" weapon sprite here
    }
}