using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D _playerRigidBody;
    public float        _playerSpeed = 5f;
    public float        _playerJumpForce = 8f;
    private Vector2      _playerDirection;

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        //_playerRigidBody.MovePosition(_playerRigidBody.position + _playerDirection.normalized * _playerSpeed * Time.fixedDeltaTime);
    }

    void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movement * Time.fixedDeltaTime * _playerSpeed;
        //_playerDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump"))
        {
            _playerRigidBody.AddForce(new Vector2 (0f, _playerJumpForce), ForceMode2D.Impulse);
        }
    }

}
