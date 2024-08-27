using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObserver : MonoBehaviour
{
    [SerializeField] private Interactable interactable;

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
        if (isTurnedOn)
        {
            print("Terminou a fase");
            SceneManager.LoadScene("Main Menu");
            // TODO - marcar que completou a fase, fazer anima��o de finaliza��o e carregar a cena de sele��o de fase
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
