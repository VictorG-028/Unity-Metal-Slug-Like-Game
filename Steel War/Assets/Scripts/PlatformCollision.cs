using System.Collections;
using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    //private GameObject _currentPlatform;
    [SerializeField] private BoxCollider2D playerCollider = null;
    [SerializeField] PlayerProperties playerProps = null;
    [Range(0.0f, 10.0f)] [SerializeField] private float dropTime = 0.5f; // Tempo durante o qual a colisão fica desativada

    void OnValidate()
    {
        if (!playerCollider) { playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>(); }
        if (!playerProps) { playerProps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerProperties>(); }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (playerProps.currentPlatform != null)
            {
                print("fez descer da plataforma");
                StartCoroutine(DisableCollision());
            }else
            {
                print("não fez nada");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //bool teste = collision.gameObject.CompareTag("Player");
        //print($"Entrou na plataforma {teste} {collision.gameObject.tag}");
        if (collision.gameObject.CompareTag("Player"))
        {
            playerProps.currentPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //bool teste = collision.gameObject.CompareTag("Player");
        //print($"Saiu da plataforma {teste} {collision.gameObject.tag}");
        if (collision.gameObject.CompareTag("Player"))
        {
            playerProps.currentPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = playerProps.currentPlatform.GetComponent<BoxCollider2D>();
        
        // Desativa a colisão para permitir que o personagem caia
        platformCollider.enabled = false;
        //Physics2D.IgnoreCollision(playerCollider, platformCollider, true);

        yield return new WaitForSeconds(dropTime);

        // Reativa a colisão após o tempo especificado
        platformCollider.enabled = true;
        //Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}
