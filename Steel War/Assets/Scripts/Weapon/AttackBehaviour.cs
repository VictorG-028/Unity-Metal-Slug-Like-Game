using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    public int damage = 1; // This is setted by any script that creates an attack

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[OnTriggerEnter2D] Bala colidiu com {other} {other.transform.name} {other.transform.tag}");
        if (other.transform.CompareTag("Player"))
        {
            return;
        }
        else if (other.transform.CompareTag("Enemy"))
        {
            EnemyProperties enemyProps = other.gameObject.GetComponent<EnemyProperties>();

            enemyProps.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("SolidGround"))
        {
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

