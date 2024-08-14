using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField] private GameObject player = null;
    [SerializeField] Animator animator = null;
    [SerializeField] PlayerProperties playerProps = null;
    [SerializeField] Camera mainCamera = null;
    [SerializeField] ParticleSystem gunShotEffect = null; // TODO: criar gameObject do efeito do tiro
    [SerializeField] GameObject gunShotGO = null;
    [SerializeField] Sprite bulletSprite = null;

    // Parameters
    [Range(3.0f, 15.0f)]
    [SerializeField] float bulletAttackDuration = 15.0f;
    [Range(0.0f, 10.0f)]
    [SerializeField] float bulletVelocity = 5.0f;


    void OnValidate()
    {
        if (!player) { player = GameObject.Find("Player"); }
        if (!animator) { animator = gameObject.GetComponent<Animator>(); }
        if (!playerProps) { playerProps = gameObject.GetComponent<PlayerProperties>(); }
        if (!mainCamera) { mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); }
        if (!gunShotEffect) { gunShotEffect = GameObject.Find("Gun Shot Effect").GetComponent<ParticleSystem>(); }
        if (!gunShotGO) { gunShotGO = GameObject.Find("Sword Slash Effect"); }
        if (!bulletSprite) { bulletSprite = Resources.Load<Sprite>("TODO"); }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // TODO verificar se tem munição da arma usada
            if (playerProps.canAttack)
            {
                ShootBullet();
            }

        }
    }

    private void ShootBullet()
    {
        playerProps.canAttack = false;

        // TODO atirar aqui
    }

    // Utils ////////////////////////////////////////////////

    private Vector3 CalculateLookDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - player.transform.position;
        return direction.normalized;
    }

    private IEnumerator MoveArrow(GameObject arrowObj)
    {
        float timer = 0f;
        //Destroy(arrowObj, arrowAttackDuration); // Destroi o Collider ap�s o tempo de ataque
        while (timer < bulletAttackDuration)
        {
            if (arrowObj == null)
                yield break; // Sai da coroutine se o objeto da flecha for destru�do prematuramente

            arrowObj.transform.position += bulletVelocity * Time.deltaTime * arrowObj.transform.forward;
            timer += Time.deltaTime;
            yield return null; // Aguarda um frame
        }
        Destroy(arrowObj);
    }
}
