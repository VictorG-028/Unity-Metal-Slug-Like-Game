using System.Collections;
using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    private GameObject _currentPlatform;
    [SerializeField] private BoxCollider2D _playerCollider;
    [SerializeField] private float dropTime = 0.5f; // Tempo durante o qual a colisão fica desativada

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (_currentPlatform != null)
            {
                StartCoroutine(DisableCollision());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            _currentPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            _currentPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D _platformCollider = _currentPlatform.GetComponent<BoxCollider2D>();

        // Desativa a colisão para permitir que o personagem caia
        Physics2D.IgnoreCollision(_playerCollider, _platformCollider, true);

        yield return new WaitForSeconds(dropTime);

        // Reativa a colisão após o tempo especificado
        Physics2D.IgnoreCollision(_playerCollider, _platformCollider, false);
    }
}
