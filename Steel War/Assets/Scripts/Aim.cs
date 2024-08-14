using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Aim : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture = null;
    [SerializeField] private GameObject player = null;
    [SerializeField] private Vector2 playerPosition = Vector2.zero;
    [SerializeField] private SpriteRenderer playerSpriteRenderer = null;
    [SerializeField] private SpriteRenderer thisSpriteRenderer = null;

    private Vector2 cursorPosition;
    private float maxUpperAngle = 90.0f;
    private float maxLowerAngle = -90.0f;

    private void OnValidate()
    {
        if (!cursorTexture) { cursorTexture = Resources.Load<Texture2D>("/Assets/Sprites/normal_sight"); } // TODO: descobrir pq isso não está carregando a imagem automaticamente
        if (!player) { player = GameObject.Find("Player"); }
        if (playerPosition == Vector2.zero) { playerPosition = GameObject.Find("Player").transform.position; }
        if (!playerSpriteRenderer) { playerSpriteRenderer = GameObject.Find("Player").GetComponent<SpriteRenderer>(); }
        if (!thisSpriteRenderer) { thisSpriteRenderer = gameObject.GetComponent<SpriteRenderer>(); }
    }

    void Start()
    {
        cursorPosition = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorPosition, CursorMode.Auto);
    }

    void Update()
    {
        Vector3 direction = CalculateLookDirection();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Ângulo entre Vector2D(1,0) e direction
        
        print(angle);

        if (angle > maxUpperAngle || angle < maxLowerAngle)
        {
            playerSpriteRenderer.flipX = true;
            thisSpriteRenderer.flipY = true;
        } 
        else
        {
            playerSpriteRenderer.flipX = false;
            thisSpriteRenderer.flipY = false;
        }

        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Utils ////////////////////////////////////////////////

    private Vector3 CalculateLookDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - player.transform.position;
        return direction.normalized;
    }
}
