using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField] PlayerProperties playerProps = null;
    // Start is called before the first frame update
    private void OnValidate()
    {
        if (!playerProps) {
            playerProps = GameObject.FindWithTag("Player").GetComponent<PlayerProperties>(); }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            playerProps.HoldOtherWeapon(2);
            Destroy(gameObject);
        }
    }
}
