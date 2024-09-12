using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObserver : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    [SerializeField] [Range(0, 3)] int shouldEndStage = 0;

    private void Start()
    {
        if (interactable != null)
        {
            // Inscreva-se para o evento
            interactable.OnStateChanged += HandleStateChanged;
        }
        else
        {
            Debug.LogWarning($"Script Observador de Interactable do GameObject {gameObject.name} n�o est� observando nada.");
        }
    }

    private void HandleStateChanged(bool isTurnedOn)
    {
        if (shouldEndStage > 0 && isTurnedOn)
        {
            print("Terminou a fase");
            SceneManager.LoadScene("Main Menu");

            // TODO - fazer anima��o de finaliza��o

            PlayerPrefs.SetInt("UnlockedLevel", 2);
            GameStateStatic.CompleteLevel(shouldEndStage);
        }
    }

    private void OnDestroy()
    {
        // Desinscreva-se do evento para evitar leaks de mem�ria
        if (interactable != null)
        {
            interactable.OnStateChanged -= HandleStateChanged;
        }
    }
}
