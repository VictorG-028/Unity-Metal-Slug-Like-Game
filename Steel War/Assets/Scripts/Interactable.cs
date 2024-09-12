using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(ParticleSystem))]
public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] Collider2D collider_2D = null;
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Sprite turnedOffSprite = null;
    [SerializeField] Sprite turnedOnSprite = null;
    [SerializeField] Sprite outlinedTurnedOffSprite = null;
    [SerializeField] Sprite outlinedTurnedOnSprite = null;
    [SerializeField] PlayerProperties playerProps = null;
    [SerializeField] Texture2D hoverCursor = null;
    [SerializeField] ParticleSystem highlightEffect = null;

    // Parameter
    [Range(0.0f, 100.0f)] [SerializeField] float minDistanceThreshold = 30.0f;
    [SerializeField] bool disableAttackWhenHover = false;

    // Control
    private bool isTurnedOn = false;
    private bool hasMouseHovering = false;

    // Collateral Effect
    public event Action<bool> OnStateChanged;

    private void OnValidate()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (!collider_2D) { collider_2D = GetComponent<Collider2D>(); }
        if (!spriteRenderer) { spriteRenderer = GetComponent<SpriteRenderer>(); }
        if (!outlinedTurnedOnSprite) { outlinedTurnedOnSprite = turnedOnSprite; }// Get rid of warning "never being used"
        //if (!turnedOnSprite && !turnedOffSprite)
        //{
        //    string[] spriteNames = {
        //        "interactables_panel_unpressed",
        //        "interactables_panel_pressed"
        //    };
        //    Sprite[] loadedSprites = loadSpritesOnValidate(
        //        "Assets/Sprites/Interactables/interactables.png",
        //        spriteNames
        //    );

        //    turnedOffSprite = loadedSprites[0];
        //    turnedOnSprite = loadedSprites[1];
        //}
        //if (!outlinedTurnedOffSprite && !outlinedTurnedOnSprite)
        //{
        //    string[] outlinedSpriteNames = {
        //        "outlined_interactables_panel_unpressed",
        //        "outlined_interactables_panel_pressed"
        //    };
        //    Sprite[] loadedSprites = loadSpritesOnValidate(
        //        "Assets/Sprites/Interactables/outlined_interactables.png",
        //        outlinedSpriteNames
        //    );

        //    outlinedTurnedOffSprite = loadedSprites[0];
        //    outlinedTurnedOnSprite = loadedSprites[1];
        //}
        if (!playerProps && player) { playerProps = player.GetComponent<PlayerProperties>(); }
        //if (!hoverCursor) { hoverCursor = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/UI/interactable_sight_cursor.png"); }
        if (!highlightEffect) { highlightEffect = GetComponent<ParticleSystem>(); }
    }

    private void Awake()
    {
        MakeSpriteMatchInternalState();

        if (transform.position.z > -1)
        {
            transform.position.Set(
                transform.position.x,
                transform.position.y,
                transform.position.z
            );
        }

        collider_2D.isTrigger = true; // Collider must be trigger
    }

    // Get & Set /////////////////////////////////////////////
    public bool IsTurnedOn
    {
        get { return isTurnedOn; }
        set
        {
            if (isTurnedOn != value)
            {
                isTurnedOn = value;
                MakeSpriteMatchInternalState();

                // Notificar os inscritos sobre a mudança de estado
                OnStateChanged?.Invoke(isTurnedOn);
            }
        }
    }

    // Implements interface /////////////////////////////////////////////

    public void OnInteract()
    {
        if (isTurnedOn) { return; }

        IsTurnedOn = true;
    }

    public bool CanInteractRestriction()
    {
        bool canInteract = true;

        float distance = Vector3.Distance(
            playerProps.transform.position, 
            this.transform.position
        );

        if (distance >= minDistanceThreshold)
        {
            canInteract = false;
        }

        return canInteract;
    }

    // Implements mouse actions (hover, click) ///////////////////////////////////////////

    private void OnMouseEnter()
    {
        //print($"mouse hover entrou em {this.name}");
        hasMouseHovering = true;

        if (disableAttackWhenHover)
        {
            playerProps.canAttack = false;
        }

        MakeSpriteMatchInternalState();
    }

    private void OnMouseExit()
    {
        //print($"mouse hover saiu de {this.name}");
        hasMouseHovering = false;

        if (disableAttackWhenHover)
        {
            playerProps.canAttack = true;
        }

        MakeSpriteMatchInternalState();
    }

    private void OnMouseDown()
    {
        print($"mouse clicou em {this.name} isTurnedOn antes é {isTurnedOn} e depois é {!isTurnedOn}");
        OnInteract();
    }

    // Utils ////////////////////////////////////////////////////////////

    private void ToggleONOff()
    {
        IsTurnedOn = !isTurnedOn;
    }

    private void MakeSpriteMatchInternalState()
    {
        if (isTurnedOn && !hasMouseHovering)
        {
            highlightEffect.Stop();
            spriteRenderer.sprite = turnedOnSprite;
            
            Cursor.SetCursor(playerProps.GetCursor, playerProps.GetHoldingWeaponCursorSize / 2, CursorMode.Auto);
        }
        else if(!isTurnedOn && !hasMouseHovering)
        {
            highlightEffect.Play();
            spriteRenderer.sprite = turnedOffSprite;
            Cursor.SetCursor(playerProps.GetCursor, playerProps.GetHoldingWeaponCursorSize / 2, CursorMode.Auto);
        }
        else if (isTurnedOn && hasMouseHovering)
        {
            highlightEffect.Stop();
            spriteRenderer.sprite = turnedOnSprite;
            Cursor.SetCursor(playerProps.GetCursor, playerProps.GetHoldingWeaponCursorSize / 2, CursorMode.Auto);
        }
        else if (!isTurnedOn && hasMouseHovering)
        {
            highlightEffect.Play();
            spriteRenderer.sprite = outlinedTurnedOffSprite;
            
            Cursor.SetCursor(hoverCursor, new Vector2(hoverCursor.width, hoverCursor.height) / 2, CursorMode.Auto);
        }
    }

    //private Sprite[] loadSpritesOnValidate(string assetPath, string[] spriteNames)
    //{
    //    Dictionary<string, int> spriteNameToIndex = enumerate(spriteNames);
    //    Sprite[] sprites = new Sprite[spriteNames.Length];
    //    UnityEngine.Object[] loadedSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

    //    foreach (UnityEngine.Object obj in loadedSprites)
    //    {
    //        if (spriteNameToIndex.Count == 0) break;

    //        if (obj is Sprite sprite && spriteNameToIndex.TryGetValue(sprite.name, out int index))
    //        {
    //            sprites[index] = sprite;
    //            spriteNameToIndex.Remove(sprite.name);
    //        }
    //    }

    //    return sprites;
    //}

    private Dictionary<string, int> enumerate(string[] names)
    {
        // Equivalente a enumerate(spriteNames) do python

        Dictionary<string, int> dict = new();
        
        for (int i = 0; i < names.Length; i++)
        {
            dict[names[i]] = i; 
        }

        return dict;
    }
}
