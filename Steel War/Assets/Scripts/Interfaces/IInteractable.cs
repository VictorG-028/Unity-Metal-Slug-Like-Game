using UnityEngine;

public interface IInteractable
{
    bool IsTurnedOn { get; set; }

    public void OnInteract() { }
    public bool CanInteractRestriction() { return true; }
}
