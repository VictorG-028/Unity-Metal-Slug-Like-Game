using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Ponte : MonoBehaviour
{
    public float moveSpeed = 2f;            // Velocidade de movimento da plataforma
    public float moveDistance = 3f;
    private Vector3 startPosition;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        // Obtém o Rigidbody2D do objeto
        rb = GetComponent<Rigidbody2D>();

        // Configura o Rigidbody2D para ser cinemático, pois queremos controlar o movimento manualmente
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Calcula a nova posição da plataforma
        Vector3 newPosition = startPosition;

        if (Input.GetKey(KeyCode.Q) && Vector3.Distance(startPosition, transform.position) < moveDistance)
        {
            // Move o objeto para a direita
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }
}
