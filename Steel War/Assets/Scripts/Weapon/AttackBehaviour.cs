using System;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    // Variables setted by any script that creates an attack
    public bool isShootedByEnemy = false;
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        string[] layersThatMakeBulletStop = {
            "SolidGround"
        };

        if (other.transform.CompareTag("Player") && isShootedByEnemy)
        {
            // TODO - fazer jogador tomar dano da bala do inimigo
            return;
        }
        else if (other.transform.CompareTag("Enemy"))
        {
            Debug.Log($"[OnTriggerEnter2D] Bala colidiu com {other} {other.transform.name} {other.transform.tag}");

            EnemyProperties enemyProps = other.gameObject.GetComponent<EnemyProperties>();

            enemyProps.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (Array.Exists(layersThatMakeBulletStop, layer => other.gameObject.layer == LayerMask.NameToLayer(layer)))
        {
            Debug.Log($"[OnTriggerEnter2D] Bala colidiu com {other} {other.transform.name} {other.gameObject.layer}");

            Destroy(gameObject);
        }
    }


    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log($"[OnCollisionEnter] Bala colidiu com {collision} {collision.transform.name} {collision.transform.tag}");
    //    if (collision.transform.CompareTag("Player"))
    //    {
    //        return;
    //    }
    //    else if (collision.transform.CompareTag("Enemy"))
    //    {
    //        EnemyProperties enemyProps = collision.gameObject.GetComponent<EnemyProperties>();

    //        enemyProps.TakeDamage(damage);
    //        Destroy(gameObject);
    //    }
    //}
}

