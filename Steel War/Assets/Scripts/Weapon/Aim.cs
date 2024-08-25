using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] private PlayerProperties playerProps = null;
    [SerializeField] private Transform playerTransform = null;
    [SerializeField] private SpriteRenderer playerSpriteRenderer = null;
    [SerializeField] private SpriteRenderer armSpriteRenderer = null;
    [SerializeField] private Transform armTransform = null;
    [SerializeField] private Transform weaponTransform = null;

    private Vector2 cursorPosition;
    private readonly float maxUpperAngle = 90.0f;
    private readonly float maxLowerAngle = -90.0f;

    private void OnValidate()
    {
        if (!playerProps) { playerProps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerProperties>(); }
        if (!playerTransform) { playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); }
        if (!playerSpriteRenderer) { playerSpriteRenderer = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>(); }
        if (!armSpriteRenderer) { armSpriteRenderer = gameObject.GetComponent<SpriteRenderer>(); }
        if (!armTransform) { armTransform = gameObject.GetComponent<Transform>(); }
        if (!weaponTransform) { weaponTransform = GameObject.Find("Weapon").GetComponent<Transform>(); }
    }

    void Awake()
    {
        cursorPosition = playerProps.GetHoldingWeaponCursorSize / 2;
        Cursor.SetCursor(playerProps.GetCursor, cursorPosition, CursorMode.Auto);
    }

    void Update()
    {
        Vector3 direction = CalculateLookDirection();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Ângulo entre Vector2D(1,0) e direction
        //print(angle);

        // Rotate Player, arm and weapon based on angle
        if (angle > maxUpperAngle || angle < maxLowerAngle)
        {
            playerTransform.rotation = Quaternion.Euler(0f, 180.0f, 0f);
            armTransform.localPosition = new Vector3(
                armTransform.localPosition.x,
                armTransform.localPosition.y,
                2
            );
            armTransform.localRotation = Quaternion.Euler(
                180.0f, 
                180.0f,
                -angle // This z rotation should update every frame, all other rotations need to be applied at least once
            );
            weaponTransform.localPosition = new Vector3(
                weaponTransform.localPosition.x,
                weaponTransform.localPosition.y,
                -1
            );

            //playerSpriteRenderer.flipX = true;
            //armSpriteRenderer.flipY = true;
            //playerProps.SetWeaponSpriteRendererFlipY(false);
            //ChangeArmXPosition(-startLocalX);
            //ChangeWeaponYPosition(correctedHandlePointY);
        }
        else
        {
            playerTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            armTransform.localPosition = new Vector3(
                armTransform.localPosition.x,
                armTransform.localPosition.y, 
                -2
            );
            armTransform.localRotation = Quaternion.Euler(
                0.0f,
                0.0f,
                angle // This z rotation should update every frame, all other rotations need to be applied at least once
            );
            weaponTransform.localPosition = new Vector3(
                weaponTransform.localPosition.x,
                weaponTransform.localPosition.y,
                1
            );

            //playerSpriteRenderer.flipX = false;
            //armSpriteRenderer.flipY = false;
            //playerProps.SetWeaponSpriteRendererFlipY(false);
            //ChangeArmXPosition(startLocalX);
            //ChangeWeaponYPosition(unchangedHandlePointY);
        }
    }

    // Utils ////////////////////////////////////////////////

    private Vector3 CalculateLookDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - playerTransform.transform.position; // weaponTransform
        return direction.normalized;
    }

    private void ChangeArmXPosition(float newLocalX)
    {
        // Explanation of code below
        // P = Player pivot point
        // x = origional x local position
        // width = player sprite width
        // ? = horizontal distance between x and x'
        // x' = x + ? = new x local position when flipY is True
        // Objetive 1: find x' for all starting x
        // Objetive 2: find x for all starting x'

        // If P is on the left {
        // P      
        // 0   x     x' width 
        // |---|-----|---|
        //
        // |---|-----|---|
        //   x    ?    x
        //
        // 2x + ? = width -> ? = width - 2x
        // x' = x + ? -> x' = x + width -2x -> x' = width - x -> x = width - x'
        // }

        // If P is on the center {
        //               P      
        // -width/2  -x  0  x' width/2 
        //        |---|--|--|---|
        //
        //        |---|--|--|---|
        //          ?  x   x  ?
        // x + ? = width/2 -> ? = width/2 - x
        // x' = -x
        // 
        // }
        transform.localPosition = new Vector3(
            newLocalX, // playerSpriteRenderer.sprite.texture.width - transform.localPosition.x
            transform.localPosition.y,
            transform.localPosition.z
        );
    }

    private void ChangeWeaponYPosition(float newLocalY)
    {
        weaponTransform.localPosition = new Vector3(
            weaponTransform.localPosition.x,
            newLocalY,
            weaponTransform.localPosition.z
        );
    }

    // ??? ////////////////////////////////////////////////
}
