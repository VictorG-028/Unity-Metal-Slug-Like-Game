using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bau : MonoBehaviour
{
    public GameObject objetoParaSpawnar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Instancia o novo objeto na posi��o e rota��o do objeto original
        Instantiate(objetoParaSpawnar, transform.position, transform.rotation);

        // Destroi o objeto original
        Destroy(gameObject);
    }
}
