using UnityEngine;
public class WeaponProperties : MonoBehaviour
{
    public Transform handlePoint = null; // Expect an child GameObject
    public Transform barrelPoint = null; // Expect an child GameObject
    public WeaponScriptable weaponScriptable  = null;

    private void Awake()
    {
        if (transform.childCount != 2)
        {
            string msg = $"The GameObject {this.name} with {this.GetType()} script must have exactly 2 child empties to define handle and barrel points.";
            throw new System.Exception(msg);
        }
    }

    private void OnValidate()
    {
        if (handlePoint == null && transform.GetChild(0))
        {
            handlePoint = transform.GetChild(0);
            
        }
        if (barrelPoint == null && transform.GetChild(1))
        {
            barrelPoint = transform.GetChild(1);
        }
    }
}