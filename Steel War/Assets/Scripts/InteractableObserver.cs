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
            Debug.LogWarning($"Script Observador de Interactable do GameObject {gameObject.name} não está observando nada.");
        }
    }

    private void HandleStateChanged(bool isTurnedOn)
    {
        if (isTurnedOn)
        {
            print("Terminou a fase");
            SceneManager.LoadScene("Main Menu");
            // TODO - marcar que completou a fase, fazer animação de finalização e carregar a cena de seleção de fase
        }
    }

    private void OnDestroy()
    {
        // Desinscreva-se do evento para evitar leaks de memória
        if (interactable != null)
        {
            interactable.OnStateChanged -= HandleStateChanged;
        }
    }
}
