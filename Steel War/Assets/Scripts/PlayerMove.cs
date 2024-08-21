using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D _playerRigidBody;
    public float _playerSpeed = 1f;
    public float _playerJumpForce = 12f;

    public int _maxJumps = 1; // Número máximo de pulos permitidos
    private int _remainingJumps; // Pulos restantes

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _remainingJumps = _maxJumps; // Inicializa com o número máximo de pulos
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movement * Time.fixedDeltaTime * _playerSpeed;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && _remainingJumps > 0)
        {
            _playerRigidBody.AddForce(new Vector2(0f, _playerJumpForce), ForceMode2D.Impulse);
            _remainingJumps--; // Decrementa o número de pulos restantes
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f) // Verifica se colidiu com o chão
        {
            _remainingJumps = _maxJumps; // Reseta o número de pulos ao tocar o chão
        }
    }
}
