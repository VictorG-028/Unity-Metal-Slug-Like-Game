using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    //[SerializeField] PlayerProperties playerProps = null;
    [SerializeField] GameObject objetoParaSpawnar = null;

    private void OnValidate()
    {
        //if (!playerProps) { playerProps = GameObject.FindWithTag("Player").GetComponent<PlayerProperties>(); }
        //if (!objetoParaSpawnar) { objetoParaSpawnar = null; } // TODO - load prefab [H] here
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Instancia o novo objeto na posição e rotação do objeto original
        GameObject newObj = Instantiate(objetoParaSpawnar, transform.position, transform.rotation);

        // Destroi o objeto original
        Destroy(gameObject);
    }
}
