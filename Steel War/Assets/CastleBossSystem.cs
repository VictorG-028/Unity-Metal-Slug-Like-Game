using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CastleBossSystem : MonoBehaviour
{
    [SerializeField] Transform targets = null;
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Animator animator = null;

    private void OnValidate()
    {
        //if (enemyProps.Length == 0) { enemyProps = transform.Find("Targets").GetComponentsInChildren<EnemyProperties>(); }
        if (!targets) { targets = transform.Find("Targets"); }
        if (!spriteRenderer) { spriteRenderer = GetComponent<SpriteRenderer>(); }
        if (!animator) { animator = GetComponent<Animator>(); }
    }

    // Parameters
    public bool shouldEndStage = false;

    // Control
    private bool isAlive = true;

    private void Update()
    {
        if (targets.childCount == 0 && isAlive)
        {
            print("Boss derrotado");
            isAlive = false;
            animator.SetBool("isDead", true);

            // TODO - trocar por anima��o de desaparecendo usando alpha
            spriteRenderer.enabled = false;

            // TODO Explosion effect
            // foreach (GameObject go in explosionGameObjects)
            // foreach (Animator a in explosionAnimators) // Hardcoded explosion
            // gameObject.enable = true;
            // animator.enable = true;
            // explosionParticleEffect.Play()

            if (shouldEndStage) StartCoroutine("EndStageAfterDelay");
        }
    }

    private IEnumerable EndStageAfterDelay()
    {
        yield return new WaitForSeconds(2.0f);
        GameStateStatic.CompleteLevel(3);
        PlayerPrefs.SetInt("UnlockedLevel", 3);
        SceneManager.LoadScene("Main menu");
    }
}