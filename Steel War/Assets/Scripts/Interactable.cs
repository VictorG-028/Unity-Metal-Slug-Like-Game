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
    [SerializeField] Collider2D collider2D = null;
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
        if (!collider2D) { collider2D = GetComponent<Collider2D>(); }
        if (!spriteRenderer) { spriteRenderer = GetComponent<SpriteRenderer>(); }

        //if (!turnedOffSprite) { turnedOffSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Interactables/interactable_small_panels.png"); }
        //if (!turnedOnSprite) { turnedOnSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Interactables/interactable_small_panels.png");  }
        if (!turnedOnSprite && !turnedOffSprite)
        {
            string[] spriteNames = {
                "interactables_panel_unpressed",
                "interactables_panel_pressed"
            };
            Sprite[] loadedSprites = loadSpritesOnValidate(
                "Assets/Sprites/Interactables/interactables.png",
                spriteNames
            );

            turnedOffSprite = loadedSprites[0];
            turnedOnSprite = loadedSprites[1];
        }
        if (!outlinedTurnedOffSprite && !outlinedTurnedOnSprite)
        {
            string[] outlinedSpriteNames = {
                "outlined_interactables_panel_unpressed",
                "outlined_interactables_panel_pressed"
            };
            Sprite[] loadedSprites = loadSpritesOnValidate(
                "Assets/Sprites/Interactables/outlined_interactables.png",
                outlinedSpriteNames
            );

            outlinedTurnedOffSprite = loadedSprites[0];
            outlinedTurnedOnSprite = loadedSprites[1];
        }
        if (!playerProps) { playerProps = GameObject.Find("Player").GetComponent<PlayerProperties>(); }
        if (!hoverCursor) { hoverCursor = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/UI/can_interact_cursor.png"); }
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

        collider2D.isTrigger = true; // Collider must be trigger
    }

    // Get & Set /////////////////////////////////////////////
    public bool IsTurnedOn
    {
        get { return isTurnedOn; }
        set { isTurnedOn = value; }
    }

    // Implements interface /////////////////////////////////////////////

    public void OnInteract()
    {
        if (isTurnedOn) { return; }

        isTurnedOn = true;
        MakeSpriteMatchInternalState();
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
        print($"mouse hover entrou em {this.name}");
        hasMouseHovering = true;

        if (disableAttackWhenHover)
        {
            playerProps.canAttack = false;
        }

        MakeSpriteMatchInternalState();
    }

    private void OnMouseExit()
    {
        print($"mouse hover saiu de {this.name}");
        hasMouseHovering = false;

        if (disableAttackWhenHover)
        {
            playerProps.canAttack = true;
        }

        MakeSpriteMatchInternalState();
    }

    private void OnMouseDown()
    {
        print($"mouse clicou em {this.name}");
        OnInteract();
    }

    // Utils ////////////////////////////////////////////////////////////

    private void ToggleONOff()
    {
        isTurnedOn = !isTurnedOn;
        MakeSpriteMatchInternalState();
    }

    private void MakeSpriteMatchInternalState()
    {
        if (isTurnedOn && !hasMouseHovering)
        {
            highlightEffect.Stop();
            spriteRenderer.sprite = turnedOnSprite;
            Cursor.SetCursor(playerProps.GetCursor, Vector2.zero, CursorMode.Auto);
        }
        else if(!isTurnedOn && !hasMouseHovering)
        {
            highlightEffect.Play();
            spriteRenderer.sprite = turnedOffSprite;
            Cursor.SetCursor(playerProps.GetCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (isTurnedOn && hasMouseHovering)
        {
            highlightEffect.Stop();
            spriteRenderer.sprite = turnedOnSprite;
            Cursor.SetCursor(playerProps.GetCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (!isTurnedOn && hasMouseHovering)
        {
            highlightEffect.Play();
            spriteRenderer.sprite = outlinedTurnedOffSprite;
            Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    private Sprite[] loadSpritesOnValidate(string assetPath, string[] spriteNames)
    {
        Dictionary<string, int> spriteNameToIndex = enumerate(spriteNames);
        Sprite[] sprites = new Sprite[spriteNames.Length];
        Object[] loadedSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

        foreach (Object obj in loadedSprites)
        {
            if (spriteNameToIndex.Count == 0) break;

            if (obj is Sprite sprite && spriteNameToIndex.TryGetValue(sprite.name, out int index))
            {
                sprites[index] = sprite;
                spriteNameToIndex.Remove(sprite.name);
            }
        }

        return sprites;
    }

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
