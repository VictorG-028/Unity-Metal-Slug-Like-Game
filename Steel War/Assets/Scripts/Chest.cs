using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject objetoParaSpawnar;
 
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Instancia o novo objeto na posi��o e rota��o do objeto original
        Instantiate(objetoParaSpawnar, transform.position, transform.rotation);

        // Destroi o objeto original
        Destroy(gameObject);
    }
}
