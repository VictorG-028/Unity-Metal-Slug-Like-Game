using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CastleBossSystem : MonoBehaviour
{
    [SerializeField] Transform targets = null;
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Animator animator = null;
    [SerializeField] ParticleSystem _particleSystem = null;

    private void OnValidate()
    {
        //if (enemyProps.Length == 0) { enemyProps = transform.Find("Targets").GetComponentsInChildren<EnemyProperties>(); }
        if (!targets) { targets = transform.Find("Targets"); }
        if (!spriteRenderer) { spriteRenderer = GetComponent<SpriteRenderer>(); }
        if (!animator) { animator = GetComponent<Animator>(); }
        if (!_particleSystem) { _particleSystem = GetComponent<ParticleSystem>(); }
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

            StartCoroutine(FadeOutSprite());
            //spriteRenderer.enabled = false;

            _particleSystem.Play();

            if (shouldEndStage)
            {
                print("Terminando estágio 3");
                //StartCoroutine(EndStageAfterDelay(true));
                EndStageAfterDelay(true);
            }
        }
    }

    private void EndStageAfterDelay(bool temp)
    {
        //yield return new WaitForSeconds(2.0f);
        GameStateStatic.CompleteLevel(3);
        PlayerPrefs.SetInt("UnlockedLevel", 3);
        SceneManager.LoadScene("Main menu");
    }

    private IEnumerator FadeOutSprite()
    {
        float fadeDuration = 2.0f;
        // Obtém a cor inicial do SpriteRenderer
        Color spriteColor = spriteRenderer.color;

        // Tempo inicial
        float startTime = Time.time;

        // Continua até que a opacidade (alfa) seja 0
        while (spriteRenderer.color.a > 0)
        {
            // Calcula o tempo passado em relação à duração do fade
            float elapsed = Time.time - startTime;

            // Calcula o valor alfa com base no tempo passado
            float alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);

            // Define a nova cor com o valor alfa ajustado
            spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);

            // Aguarda o próximo frame antes de continuar
            yield return null;
        }

        // Garante que o alfa seja exatamente 0 no final
        spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0);
    }
}
